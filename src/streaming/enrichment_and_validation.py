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
    .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(args.storage_account_name),
         args.storage_account_key)

spark = SparkSession\
    .builder\
    .config(conf=spark_conf)\
    .getOrCreate()

sc = spark.sparkContext
print("Spark Configuration:")
_ = [print(k + '=' + v) for k, v in sc.getConf().getAll()]

# %%
BASE_STORAGE_PATH = "wasbs://{0}@{1}.blob.core.windows.net/".format(
    args.storage_container_name, args.storage_account_name
)

print("Base storage url:", BASE_STORAGE_PATH)

# %%

from geh_stream.schemas import SchemaFactory, SchemaNames
from pyspark.sql.functions import coalesce, lit, col

master_data_storage_path = BASE_STORAGE_PATH + args.master_data_path

csv_read_config = {
    "inferSchema": "True",
    "delimiter": ",",
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
    'eventhubs.maxEventsPerTrigger': args.max_events_per_trigger,
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
parsed_data = EventHubParser.parse(raw_data, message_schema)

print("Parsed stream schema:")
parsed_data.printSchema()

# %%
from geh_stream.streaming_utils import Enricher

enriched_data = Enricher.enrich(parsed_data, master_data)

print("Enriched stream schema:")
enriched_data.printSchema()

# %%
from geh_stream.validation import Validator

validated_data = Validator.validate(enriched_data)
print("Validated stream schema:")
validated_data.printSchema()

# %%
from pyspark.sql import DataFrame

import geh_stream.batch_operations as batch_operations

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
    batch_df.persist()

    # Make valid time series points available to aggregations (by storing in Delta lake)
    batch_operations.store_valid_data(batch_df, output_delta_lake_path)

    # Forward all valid time series points to message shipping (by sending to Kafka topic)
    batch_operations.send_valid_data(batch_df, valid_output_eh_conf)

    # Forward all invalid time series points to further processing like sending
    # a negative acknowledgement to the sending market actor.
    batch_operations.send_invalid_data(batch_df, invalid_output_eh_conf)

    batch_df.unpersist()


out_stream = validated_data \
    .writeStream \
    .option("checkpointLocation", checkpoint_path) \
    .trigger(processingTime=args.trigger_interval) \
    .foreachBatch(__store_data_frame)

# %%
import time

while True:
    execution = out_stream.start()
    time.sleep(4.5 * 60)
