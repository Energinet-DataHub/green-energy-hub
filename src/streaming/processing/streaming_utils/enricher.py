from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col


class Enricher:

    @staticmethod
    def enrich(parsed_data: DataFrame, master_data: DataFrame):
        return parsed_data.alias("pd") \
            .join(master_data.alias("md"),
                  (col("pd.MarketEvaluationPoint_mRID") == col("md.MarketEvaluationPoint_mRID"))
                  & col("ObservationTime").between(col("md.ValidFrom"), col("md.ValidTo")), how="left") \
            .drop(master_data["MarketEvaluationPoint_mRID"]) \
            .drop(master_data["ValidFrom"]).drop(master_data["ValidTo"])
