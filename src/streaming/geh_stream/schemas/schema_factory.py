# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
import copy
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import col, from_json
from pyspark.sql.types import StringType, StructType, StructField, TimestampType, DecimalType, ArrayType
from .schema_names import SchemaNames


# See NOTE on usage
def make_all_nullable(schema):
    schema.nullable = True
    if isinstance(schema, StructField):
        make_all_nullable(schema.dataType)
    if isinstance(schema, ArrayType):
        make_all_nullable(schema.elementType)
    if isinstance(schema, StructType):
        for f in schema.fields:
            make_all_nullable(f)


class SchemaFactory:

    message_body_schema: StructType = StructType() \
        .add("mRID", StringType(), False) \
        .add("MessageReference", StringType(), False) \
        .add("MarketDocument", StructType()
             .add("mRID", StringType(), False)
             .add("Type", StringType(), False)
             .add("CreatedDateTime", TimestampType(), False)
             .add("SenderMarketParticipant", StructType()
                  .add("mRID", StringType(), False)
                  .add("Type", StringType(), False), False)
             .add("RecipientMarketParticipant", StructType()
                  .add("mRID", StringType(), False)
                  .add("Type", StringType(), True), False)
             .add("ProcessType", StringType(), False)
             .add("MarketServiceCategory_Kind", StringType(), False), False) \
        .add("MktActivityRecord_Status", StringType(), False) \
        .add("Product", StringType(), False) \
        .add("QuantityMeasurementUnit_Name", StringType(), False) \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType(), True) \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("CorrelationId", StringType(), False) \
        .add("Period", StructType()
             .add("Resolution", StringType(), False)
             .add("TimeInterval_Start", TimestampType(), False)
             .add("TimeInterval_End", TimestampType(), False)
             .add("Points", ArrayType(StructType()
                  .add("Quantity", DecimalType(), False)
                  .add("Quality", StringType(), False)
                  .add("ObservationTime", TimestampType(), False), True), False), False)

    # ValidFrom and ValidTo are not to be included in outputs from the time series point streaming process
    master_schema: StructType = StructType() \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("ValidFrom", TimestampType(), False) \
        .add("ValidTo", TimestampType(), True) \
        .add("MeterReadingPeriodicity", StringType(), False) \
        .add("MeteringMethod", StringType(), False) \
        .add("MeteringGridArea_Domain_mRID", StringType(), True) \
        .add("ConnectionState", StringType(), True) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType(), True) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), True) \
        .add("InMeteringGridArea_Domain_mRID", StringType(), False) \
        .add("InMeteringGridArea_Domain_Owner_mRID", StringType(), False) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), False) \
        .add("OutMeteringGridArea_Domain_Owner_mRID", StringType(), False) \
        .add("Parent_Domain_mRID", StringType(), False) \
        .add("ServiceCategory_Kind", StringType(), False) \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType(), False) \
        .add("QuantityMeasurementUnit_Name", StringType(), False) \
        .add("Product", StringType(), False) \
        .add("Technology", StringType(), True)

    parsed_schema = copy.deepcopy(message_body_schema)
    # NOTE: This is a workaround because for some unknown reason pyspark parsing from JSON
    #       (in event_hub_parser.py) causes all to be nullable regardless of the schema
    make_all_nullable(parsed_schema)

    parquet_schema: StructType = StructType() \
        .add("CorrelationId", StringType(), False) \
        .add("MessageReference", StringType(), False) \
        .add("MarketDocument_mRID", StringType(), False) \
        .add("CreatedDateTime", TimestampType(), False) \
        .add("SenderMarketParticipant_mRID", StringType(), False) \
        .add("ProcessType", StringType(), False) \
        .add("SenderMarketParticipantMarketRole_Type", StringType(), False) \
        .add("MarketServiceCategory_Kind", StringType(), False) \
        .add("TimeSeries_mRID", StringType(), False) \
        .add("MktActivityRecord_Status", StringType(), False) \
        .add("Product", StringType(), False) \
        .add("QuantityMeasurementUnit_Name", StringType(), False) \
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
        .add("EnergySupplier_MarketParticipant_mRID", StringType(), False) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), False) \
        .add("InMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("Parent_Domain_mRID", StringType(), True) \
        .add("ServiceCategory_Kind", StringType(), True) \
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
