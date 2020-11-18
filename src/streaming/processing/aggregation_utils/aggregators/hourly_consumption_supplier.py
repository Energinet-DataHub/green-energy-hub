from pyspark.sql import DataFrame
from pyspark.sql.functions import col, window
from processing.codelists import MarketEvaluationPointType, SettlementMethod


class HourlyConsumptionSupplierAggregator:

    @staticmethod
    def aggregate(df: DataFrame):
        return df \
            .filter(col("MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value) \
            .filter(col("SettlementMethod") == SettlementMethod.non_profiled.value) \
            .groupBy("MeteringGridArea_Domain_mRID",
                     "BalanceResponsibleParty_MarketParticipant_mRID",
                     "EnergySupplier_MarketParticipant_mRID",
                     window(col("ObservationTime"), "1 hour")) \
            .sum("Quantity") \
            .withColumnRenamed("sum(Quantity)", "sum_quantity") \
            .withColumnRenamed("window", "time_window") \
            .orderBy("MeteringGridArea_Domain_mRID",
                     "BalanceResponsibleParty_MarketParticipant_mRID",
                     "EnergySupplier_MarketParticipant_mRID",
                     "time_window")
