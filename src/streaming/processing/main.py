"""
Data ingestion stream
"""

# TODO: consider using pyspark-stubs=3.0.0 and mypy
# %%
import json
import time
import urllib.parse

import configargparse

from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import StructType, TimestampType, StringType, DoubleType
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit

# %%
p = configargparse.ArgParser(prog='main.py', description='Green Energy Hub Streaming',
    default_config_files=['run_args.conf'],
    formatter_class=configargparse.ArgumentDefaultsHelpFormatter
)
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
p.add('--output-eh-connection-string', type=str, required=True,
    help='Output Event Hub connection string', env_var='GEH_STREAMING_OUTPUT_EH_CONNECTION_STRING')

args, unknown_args = p.parse_known_args()

if unknown_args:
    print("Unknown args:")
    _ = [print(arg) for arg in unknown_args]

# %%
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

master_data_storage_path = BASE_STORAGE_PATH + args.master_data_path

csv_read_config = {
    "inferSchema": "True",
    "delimiter": ",",
    "header": "True",
    "nullValues": "NULL"
}
master_data_schema = StructType() \
    .add("MarketEvaluationPoint_mRID", StringType(), False) \
    .add("ValidFrom", TimestampType(), False) \
    .add("ValidTo", TimestampType(), True) \
    .add("MeterReadingPeriodicity", StringType(), False) \
    .add("MeterReadingPeriodicity2", StringType(), False) \
    .add("MeteringMethod", StringType(), False) \
    .add("MeteringGridArea_Domain_mRID", StringType(), True) \
    .add("ConnectionState", StringType(), True) \
    .add("EnergySupplier_MarketParticipant_mRID", StringType(), False) \
    .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), False) \
    .add("InMeteringGridArea_Domain_mRID", StringType(), False) \
    .add("OutMeteringGridArea_Domain_mRID", StringType(), False) \
    .add("Parent_Domain", StringType(), False) \
    .add("SupplierAssociationId", StringType(), False) \
    .add("ServiceCategoryKind", StringType(), False) \
    .add("MarketEvaluationPointType", StringType(), False) \
    .add("SettlementMethod", StringType(), False) \
    .add("UnitName", StringType(), False) \
    .add("Product", StringType(), False)

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

message_schema: StructType = StructType() \
    .add("MarketEvaluationPoint_mRID", StringType(), False) \
    .add("ObservationTime", TimestampType(), False) \
    .add("Quantity", DoubleType(), True) \
    .add("CorrelationId", StringType(), True) \
    .add("MessageReference", StringType(), True) \
    .add("HeaderEnergyDocument_mRID", StringType(), True) \
    .add("HeaderEnergyDocumentCreation", TimestampType(), True) \
    .add("HeaderEnergyDocumentSenderIdentification", StringType(), True) \
    .add("EnergyBusinessProcess", StringType(), True) \
    .add("EnergyBusinessProcessRole", StringType(), True) \
    .add("TimeSeriesmRID", StringType(), True) \
    .add("MktActivityRecord_Status", StringType(), True) \
    .add("Product", StringType(), True) \
    .add("UnitName", StringType(), True) \
    .add("MarketEvaluationPointType", StringType(), True) \
    .add("Quality", StringType(), True)

parsed_data = raw_data \
    .select(
        col("enqueuedTime"),
        from_json(col("body").cast("string"), message_schema).alias("message")
    ) \
    .select(col("message.*"), col("enqueuedTime").alias("EventHubEnqueueTime"))

print("Parsed stream schema:")
parsed_data.printSchema()

# %%
# TODO: remove column repetitions (after validation?)
enriched_data = parsed_data.alias("pd") \
    .join(master_data.alias("md"),
          (col("pd.MarketEvaluationPoint_mRID") == col("md.MarketEvaluationPoint_mRID")) &
          col("ObservationTime").between(
              col("md.ValidFrom"), col("md.ValidTo")),
          how="left"
          ) \
    .drop(master_data["MarketEvaluationPoint_mRID"]) \
    .drop(master_data["ValidFrom"]).drop(master_data["ValidTo"])


print("Enriched stream schema:")
enriched_data.printSchema()

# %%
validated_data = enriched_data \
    .withColumn("IsValid",
                # Streamed market evaluation point type must match master data
                (col("md.MarketEvaluationPointType").isNotNull()
                 & (col("pd.MarketEvaluationPointType") == col("md.MarketEvaluationPointType")))
                # Quantity must be null or non-negative
                & (col("Quantity").isNull() | (col("Quantity") >= 0))
    ) \
    .drop(parsed_data["UnitName"]).drop(parsed_data["Product"]) \
    .drop(parsed_data["MarketEvaluationPointType"])
    
print("Validated stream schema:")
validated_data.printSchema()

# %%
output_eh_connection_string = args.output_eh_connection_string
output_delta_lake_path = BASE_STORAGE_PATH + args.output_path
checkpoint_path = BASE_STORAGE_PATH + args.streaming_checkpoint_path

output_eh_conf = {
    'eventhubs.connectionString': \
        sc._jvm.org.apache.spark.eventhubs.EventHubsUtils.encrypt(output_eh_connection_string),
}


def __store_data_frame(batch_df: DataFrame, _: int):
    batch_df.persist()

    batch_df \
        .filter(col("IsValid") == lit(True)) \
        .select(col("*"),
                year("ObservationTime").alias("year"),
                month("ObservationTime").alias("month"),
                dayofmonth("ObservationTime").alias("day")) \
        .repartition("year", "month", "day") \
        .write \
        .partitionBy("year", "month", "day") \
        .format("delta") \
        .mode("append") \
        .save(output_delta_lake_path)

    batch_df \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_eh_conf) \
        .save()

    batch_df.unpersist()


out_stream = validated_data \
    .writeStream \
    .option("checkpointLocation", checkpoint_path) \
    .trigger(processingTime=args.trigger_interval) \
    .foreachBatch(__store_data_frame)

# %%
while True:
    execution = out_stream.start()
    time.sleep(4.5 * 60)
