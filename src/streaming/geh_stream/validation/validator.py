from pyspark.sql import DataFrame, SparkSession
from .rules import rules


class Validator:

    @staticmethod
    def validate(enriched_data: DataFrame):
        validated_data = enriched_data

        for rule in rules:
            validated_data = rule(validated_data)

        return validated_data
