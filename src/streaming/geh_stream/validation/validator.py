from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col
from .rules import rules


class Validator:

    @staticmethod
    def validate(enriched_data: DataFrame):
        validated_data = enriched_data

        for rule in rules:
            validated_data = rule(validated_data)

        # Dropped columns are duplicates (from both streamed data and master data).
        # They should not be necessary after validation.
        return validated_data \
            .withColumn("IsValid",
                        col("VR-245-1-Is-Valid")
                        & col("VR-250-Is-Valid")
                        & col("VR-251-Is-Valid")
                        & col("VR-611-Is-Valid")
                        & col("VR-612-Is-Valid")) \
            .drop(col("pd.MarketEvaluationPointType")) \
            .drop(col("pd.QuantityMeasurementUnit_Name")) \
            .drop(col("pd.Product"))
