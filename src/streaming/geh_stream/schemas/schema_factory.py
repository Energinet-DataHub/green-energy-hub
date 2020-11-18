import copy
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col, from_json
from pyspark.sql.types import DoubleType, StringType, StructType, TimestampType, DecimalType
from .schema_names import SchemaNames


class SchemaFactory:
    message_body_schema: StructType = StructType() \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("ObservationTime", TimestampType(), False) \
        .add("Quantity", DoubleType(), True) \
        .add("CorrelationId", StringType(), True) \
        .add("MessageReference", StringType(), True) \
        .add("HeaderEnergyDocument_mRID", StringType(), True) \
        .add("HeaderEnergyDocumentCreation", TimestampType(), True) \
        .add("HeaderEnergyDocumentSenderIdentification", StringType(), True) \
        .add("EnergyBusinessProcess", StringType(), True) \
        .add("SenderMarketParticipantMarketRoleType", StringType(), True) \
        .add("TimeSeriesmRID", StringType(), True) \
        .add("MktActivityRecord_Status", StringType(), True) \
        .add("Product", StringType(), True) \
        .add("UnitName", StringType(), True) \
        .add("MarketEvaluationPointType", StringType(), True) \
        .add("Quality", StringType(), True)

    master_schema: StructType = StructType() \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("ValidFrom", TimestampType(), False) \
        .add("ValidTo", TimestampType(), True) \
        .add("MeterReadingPeriodicity", StringType(), False) \
        .add("MeteringMethod", StringType(), False) \
        .add("MeteringGridArea_Domain_mRID", StringType(), True) \
        .add("ConnectionState", StringType(), True) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType(), False) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), False) \
        .add("InMeteringGridArea_Domain_mRID", StringType(), False) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), False) \
        .add("Parent_Domain", StringType(), False) \
        .add("ServiceCategoryKind", StringType(), False) \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType(), False) \
        .add("UnitName", StringType(), False) \
        .add("Product", StringType(), False)

    parsed_schema = copy.deepcopy(message_body_schema).add("EventHubEnqueueTime", TimestampType(), False)

    parquet_schema: StructType = StructType() \
        .add("CorrelationId", StringType(), False) \
        .add("MessageReference", StringType(), False) \
        .add("MarketDocument_mRID", StringType(), False) \
        .add("CreatedDateTime", TimestampType(), False) \
        .add("SenderMarketParticipant_mRID", StringType(), False) \
        .add("ProcessType", StringType(), False) \
        .add("SenderMarketParticipantMarketRoleType", StringType(), False) \
        .add("MarketServiceCategory_Kind", StringType(), False) \
        .add("TimeSeries_mRID", StringType(), False) \
        .add("MktActivityRecord_Status", StringType(), False) \
        .add("Product", StringType(), False) \
        .add("UnitName", StringType(), False) \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType(), True) \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("Quantity", DecimalType(), True) \
        .add("Quality", StringType(), True) \
        .add("ObservationTime", TimestampType(), False) \
        .add("MeteringMethod", StringType(), True) \
        .add("MeterReadingPeriodicity", StringType(), True) \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("ConnectionState", StringType(), False) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType(), True) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), True) \
        .add("InMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("Parent_Domain", StringType(), True) \
        .add("ServiceCategoryKind", StringType(), True) \
        .add("Technology", StringType(), True)

    # For right now, this is the simplest solution for getting master/parsed data
    # This should be improved
    @staticmethod
    def get_instance(schema_name: SchemaNames):
        if schema_name is SchemaNames.Parsed:
            return SchemaFactory.parsed_schema
        elif schema_name is SchemaNames.Master:
            return SchemaFactory.master_schema
        elif schema_name is SchemaNames.MessageBody:
            return SchemaFactory.message_body_schema
        elif schema_name is SchemaNames.Parquet:
            return SchemaFactory.parquet_schema
        else:
            return None
