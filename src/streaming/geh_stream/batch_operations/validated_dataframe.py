from pyspark.sql import DataFrame
from pyspark.sql.types import StructType, TimestampType, StringType, DoubleType
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit


def send_valid_data(batch_df: DataFrame, output_eh_conf):
    batch_df \
        .filter(col("IsValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("ObservationTime"),
                col("Quantity"),
                col("CorrelationId"),
                col("MessageReference"),
                col("HeaderEnergyDocument_mRID"),
                col("HeaderEnergyDocumentCreation"),
                col("HeaderEnergyDocumentSenderIdentification"),
                col("ProcessType"),
                col("SenderMarketParticipantMarketRoleType"),
                col("TimeSeriesmRID"),
                col("MktActivityRecord_Status"),
                col("MarketEvaluationPointType"),
                col("Quality"),
                col("MeterReadingPeriodicity"),
                col("MeteringMethod"),
                col("MeteringGridArea_Domain_mRID"),
                col("ConnectionState"),
                col("EnergySupplier_MarketParticipant_mRID"),
                col("BalanceResponsibleParty_MarketParticipant_mRID"),
                col("InMeteringGridArea_Domain_mRID"),
                col("OutMeteringGridArea_Domain_mRID"),
                col("Parent_Domain"),
                col("ServiceCategoryKind"),
                col("SettlementMethod"),
                col("UnitName"),
                col("Product"),
                col("ObservationTime")) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_eh_conf) \
        .save()


def send_invalid_data(batch_df: DataFrame, output_invalid_eh_conf):
    batch_df \
        .filter(col("IsValid") == lit(False)) \
        .select(col("ProcessType"),
                col("SenderMarketParticipantMarketRoleType"),
                col("HeaderEnergyDocumentSenderIdentification"),
                col("HeaderEnergyDocument_mRID"),
                col("MktActivityRecord_Status"),
                col("VR-245-1-Is-Valid"),
                col("VR-250-Is-Valid"),
                col("VR-251-Is-Valid"),
                col("VR-611-Is-Valid"),
                col("VR-612-Is-Valid")) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_invalid_eh_conf) \
        .save()


def store_valid_data(batch_df: DataFrame, output_delta_lake_path):
    batch_df \
        .filter(col("IsValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("ObservationTime"),
                col("Quantity"),
                col("CorrelationId"),
                col("MessageReference"),
                col("HeaderEnergyDocument_mRID"),
                col("HeaderEnergyDocumentCreation"),
                col("HeaderEnergyDocumentSenderIdentification"),
                col("ProcessType"),
                col("SenderMarketParticipantMarketRoleType"),
                col("TimeSeriesmRID"),
                col("MktActivityRecord_Status"),
                col("MarketEvaluationPointType"),
                col("Quality"),
                col("MeterReadingPeriodicity"),
                col("MeteringMethod"),
                col("MeteringGridArea_Domain_mRID"),
                col("ConnectionState"),
                col("EnergySupplier_MarketParticipant_mRID"),
                col("BalanceResponsibleParty_MarketParticipant_mRID"),
                col("InMeteringGridArea_Domain_mRID"),
                col("OutMeteringGridArea_Domain_mRID"),
                col("Parent_Domain"),
                col("ServiceCategoryKind"),
                col("SettlementMethod"),
                col("UnitName"),
                col("Product"),

                year("ObservationTime").alias("year"),
                month("ObservationTime").alias("month"),
                dayofmonth("ObservationTime").alias("day")) \
        .repartition("year", "month", "day") \
        .write \
        .partitionBy("year", "month", "day") \
        .format("delta") \
        .mode("append") \
        .save(output_delta_lake_path)
