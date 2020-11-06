from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col

class Validator:

    @staticmethod
    def validate(enriched_data: DataFrame):
        validated_data = enriched_data \
            .withColumn("IsValid", \
                # Streamed market evaluation point type must match master data
                (col("md.MarketEvaluationPointType").isNotNull() \
                & (col("pd.MarketEvaluationPointType") == col("md.MarketEvaluationPointType"))) \
                # Quantity must be null or non-negative
                & (col("Quantity").isNull() | (col("Quantity") >= 0)))

        return validated_data