import copy
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col, from_json
from pyspark.sql.types import DoubleType, StringType, StructType, TimestampType
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
        .add("EnergyBusinessProcessRole", StringType(), True) \
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

    parsed_schema = copy.deepcopy(message_body_schema).add("EventHubEnqueueTime", TimestampType(), False)

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
        else:
            return None
