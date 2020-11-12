from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col, from_json
from pyspark.sql.types import StructType


class EventHubParser:

    @staticmethod
    def parse(raw_data: DataFrame, message_schema: StructType):
        return raw_data \
            .select(col("enqueuedTime"), from_json(col("body").cast("string"), message_schema).alias("message")) \
            .select(col("message.*"), col("enqueuedTime").alias("EventHubEnqueueTime"))
