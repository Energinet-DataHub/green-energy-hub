# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""
Data ingestion stream
"""

# TODO: consider using pyspark-stubs=3.0.0 and mypy

# %%
import configargparse

p = configargparse.ArgParser(prog='enrichment_and_validation.py',
                             description='Green Energy Hub Streaming',
                             default_config_files=['configuration/run_args_enrichment_and_validation.conf'],
                             formatter_class=configargparse.ArgumentDefaultsHelpFormatter)
p.add('--storage-account-name', type=str, required=True,
      help='Azure Storage account name (used for data output and checkpointing)')
p.add('--storage-account-key', type=str, required=True,
      help='Azure Storage key', env_var='GEH_STREAMING_STORAGE_KEY')
p.add('--storage-container-name', type=str, required=False, default='data',
      help='Azure Storage container name')
p.add('--master-data-path', type=str, required=False, default="master-data/MasterData.csv",
      help='Path to master data storage location (csv) relative to container''s root')
p.add('--output-path', type=str, required=False, default="delta/meter-data/",
      help='Path to stream output storage location (deltalake) relative to container''s root')
p.add('--input-eh-connection-string', type=str, required=True,
      help='Input Event Hub connection string', env_var='GEH_STREAMING_INPUT_EH_CONNECTION_STRING')
p.add('--max-events-per-trigger', type=int, required=False, default=10000,
      help='Metering points to read per trrigger interval')
p.add('--trigger-interval', type=str, required=False, default='1 second',
      help='Trigger interval to generate streaming batches (format: N seconds)')
p.add('--streaming-checkpoint-path', type=str, required=False, default="checkpoints/streaming",
      help='Path to checkpoint folder for streaming')
p.add('--valid-output-eh-connection-string', type=str, required=True,
      help='Output Event Hub connection string for valid time series points', env_var='GEH_STREAMING_VALID_OUTPUT_EH_CONNECTION_STRING')
p.add('--invalid-output-eh-connection-string', type=str, required=True,
      help='Output Event Hub connection string for invalid time series points', env_var='GEH_STREAMING_INVALID_OUTPUT_EH_CONNECTION_STRING')
p.add('--telemetry-instrumentation-key', type=str, required=True,
      help='Instrumentation key used for telemetry')

args, unknown_args = p.parse_known_args()

if unknown_args:
    print("Unknown args:")
    _ = [print(arg) for arg in unknown_args]

# %%
from pyspark import SparkConf
from pyspark.sql import SparkSession

spark_conf = SparkConf(loadDefaults=True) \
    .set('fs.azure.account.key.{0}.dfs.core.windows.net'.format(args.storage_account_name),
         args.storage_account_key)

spark = SparkSession\
    .builder\
    .config(conf=spark_conf)\
    .getOrCreate()

sc = spark.sparkContext
print("Spark Configuration:")
_ = [print(k + '=' + v) for k, v in sc.getConf().getAll()]

# %%
BASE_STORAGE_PATH = "abfss://{0}@{1}.dfs.core.windows.net/".format(
    args.storage_container_name, args.storage_account_name
)

print("Base storage url:", BASE_STORAGE_PATH)

# %%

from geh_stream.schemas import SchemaFactory, SchemaNames
from pyspark.sql.functions import coalesce, lit, col

master_data_storage_path = BASE_STORAGE_PATH + args.master_data_path

csv_read_config = {
    "inferSchema": "True",
    "delimiter": ";",
    "header": "True",
    "nullValues": "NULL"
}

master_data_schema = SchemaFactory.get_instance(SchemaNames.Master)

master_data = spark \
    .read \
    .format("csv") \
    .schema(master_data_schema) \
    .options(**csv_read_config) \
    .load(master_data_storage_path)

master_data = master_data \
    .withColumn("ValidTo",
                coalesce(col("ValidTo"), lit("9999-12-31").cast("timestamp")))

master_data.printSchema()
master_data.show()

# %%
import json

input_eh_starting_position = {
    "offset": "-1",         # starting from beginning of stream
    "seqNo": -1,            # not in use
    "enqueuedTime": None,   # not in use
    "isInclusive": True
}
input_eh_connection_string = args.input_eh_connection_string
input_eh_conf = {
    # Version 2.3.15 and up requires encryption
    'eventhubs.connectionString': \
    sc._jvm.org.apache.spark.eventhubs.EventHubsUtils.encrypt(input_eh_connection_string),
    'eventhubs.startingPosition': json.dumps(input_eh_starting_position),
    'eventhubs.prefetchCount': 5000,
    'maxEventsPerTrigger': args.max_events_per_trigger,
}

print("Input event hub config:", input_eh_conf)

# %%
raw_data = spark \
    .readStream \
    .format("eventhubs") \
    .options(**input_eh_conf) \
    .option("inferSchema", True) \
    .load()

print("Input stream schema:")
raw_data.printSchema()

# %%
from pyspark.sql.types import StructType

from geh_stream.streaming_utils import EventHubParser
from geh_stream.schemas import SchemaFactory, SchemaNames

message_schema: StructType = SchemaFactory.get_instance(SchemaNames.MessageBody)

# Event hub message parser function
parsed_data = EventHubParser.parse(raw_data, message_schema) \
    .withColumnRenamed("mRID", "TimeSeries_mRID")

print("Parsed stream schema:")
parsed_data.printSchema()

# %% Denormalize messages: Flatten and explode messages by each contained time series point
from geh_stream.streaming_utils.denormalization import denormalize_parsed_data

denormalized_data = denormalize_parsed_data(parsed_data)

print("denormalized_data schema")
denormalized_data.printSchema()

# %%
from geh_stream.streaming_utils import Enricher

enriched_data = Enricher.enrich(denormalized_data, master_data)

print("Enriched stream schema:")
enriched_data.printSchema()

# %%
from geh_stream.validation import Validator

validated_data = Validator.add_validation_status_columns(enriched_data)

print("Validated stream schema:")
validated_data.printSchema()

# %%
from pyspark.sql import DataFrame

from geh_stream.monitoring import Telemetry, MonitoredStopwatch
import geh_stream.batch_operations as batch_operations

telemetry_client = Telemetry.create_telemetry_client(args.telemetry_instrumentation_key)

valid_output_eh_connection_string = args.valid_output_eh_connection_string
invalid_output_eh_connection_string = args.invalid_output_eh_connection_string
output_delta_lake_path = BASE_STORAGE_PATH + args.output_path
checkpoint_path = BASE_STORAGE_PATH + args.streaming_checkpoint_path

# Event Hub for valid times series points
valid_output_eh_conf = {
    'eventhubs.connectionString':
    sc._jvm.org.apache.spark.eventhubs.EventHubsUtils.encrypt(valid_output_eh_connection_string),
}

# Event Hub for invalid time series points
invalid_output_eh_conf = {
    'eventhubs.connectionString':
    sc._jvm.org.apache.spark.eventhubs.EventHubsUtils.encrypt(invalid_output_eh_connection_string),
}


def __store_data_frame(batch_df: DataFrame, _: int):
    try:
        watch = MonitoredStopwatch.start_timer(telemetry_client, "StoreDataFrame")

        persist_timer = watch.start_sub_timer("persist")
        # Cache the batch in order to avoid the risk of recalculation in each write operation
        batch_df = batch_df.persist()
        persist_timer.stop_timer()

        correlation_ids = batch_operations.get_involved_correlation_ids(batch_df, watch)
        batch_count = batch_operations.get_rows_in_batch(batch_df, watch)

        # Make valid time series points available to aggregations (by storing in Delta lake)
        batch_operations.store_valid_data(batch_df, output_delta_lake_path, watch)

        # Forward all valid time series points to message shipping (by sending to Kafka topic)
        batch_operations.send_valid_data(batch_df, valid_output_eh_conf, watch)

        # Forward all invalid time series points to further processing like sending
        # a negative acknowledgement to the sending market actor.
        batch_operations.send_invalid_data(batch_df, invalid_output_eh_conf, watch)

        unpersist_timer = watch.start_sub_timer("unpersist")
        batch_df = batch_df.unpersist()
        unpersist_timer.stop_timer()

        watch.stop_timer(batch_count)

        batch_operations.track_batch_back_to_original_correlation_requests(correlation_ids, batch_count, telemetry_client, watch)

    except Exception as err:
        # Make sure the exception is not accidently tracked on the last used parent
        telemetry_client.context.operation.parent_id = None
        # We need to track and flush the exception so it is not lost in case the exception will stop execution
        telemetry_client.track_exception()
        telemetry_client.flush()
        # The exception needs to continue its journey as to not cause data loss
        raise err


# checkpointLocation is used to support failure (or intentional shut-down)
# recovery with a exactly-once semantic. See more on
# https://spark.apache.org/docs/latest/structured-streaming-programming-guide.html#fault-tolerance-semantics.
# The trigger determines how often a batch is created and processed.
out_stream = validated_data \
    .writeStream \
    .option("checkpointLocation", checkpoint_path) \
    .trigger(processingTime=args.trigger_interval) \
    .foreachBatch(__store_data_frame)

# %%
import time

# Purpose of this construction is to ensure that the master data used to enrich the streaming data
# is no older than 5 minutes.
# The alternative of restarting the job every 5ish minutes was considered too expensive.
# The assumption here is that reading of master data and restart of the streaming
# can be done in less than 30 seconds.
while True:
    print("(Re)start streaming with fresh master data.")

    # Persist master data to avoid rereading them in each batch
    master_data.persist()

    execution = out_stream.start()
    time.sleep(4.5 * 60)
    master_data.unpersist()
