import copy
import pytest
import pandas as pd
import time
from processing.validation import Validator
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import BooleanType, DoubleType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col, lit

# Create timestamps used in DataFrames
time_now = time.time()
timestamp_now = pd.Timestamp(time_now, unit='s')


# Schemas for master and parsed data that becomes the enriched data via join
@pytest.fixture(scope="module")
def parsed_data_schema():
    print("PARSED DATA SCHEMA")
    return StructType() \
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
        .add("Quality", StringType(), True) \
        .add("EventHubEnqueueTime", TimestampType(), False)


@pytest.fixture(scope="module")
def master_data_schema():
    return StructType() \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
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


# Expected schema of the resulting Dataframe from the validate function
# To get the expected schema, we cannot combine the above schemas in code because the join
# makes fields nullable and columns are also dropped.
@pytest.fixture(scope="module")
def expected_validate_schema():
    return StructType() \
        .add(StructField("MarketEvaluationPoint_mRID", StringType(), False)) \
        .add(StructField("ObservationTime", TimestampType(), False)) \
        .add(StructField("Quantity", DoubleType(), True)) \
        .add(StructField("CorrelationId", StringType(), True)) \
        .add(StructField("MessageReference", StringType(), True)) \
        .add(StructField("HeaderEnergyDocument_mRID", StringType(), True)) \
        .add(StructField("HeaderEnergyDocumentCreation", TimestampType(), True)) \
        .add(StructField("HeaderEnergyDocumentSenderIdentification", StringType(), True)) \
        .add(StructField("EnergyBusinessProcess", StringType(), True)) \
        .add(StructField("EnergyBusinessProcessRole", StringType(), True)) \
        .add(StructField("TimeSeriesmRID", StringType(), True)) \
        .add(StructField("MktActivityRecord_Status", StringType(), True)) \
        .add(StructField("Product", StringType(), True)) \
        .add(StructField("UnitName", StringType(), True)) \
        .add(StructField("MarketEvaluationPointType", StringType(), True)) \
        .add(StructField("Quality", StringType(), True)) \
        .add(StructField("EventHubEnqueueTime", TimestampType(), False)) \
        .add(StructField("MeterReadingPeriodicity", StringType(), True)) \
        .add(StructField("MeterReadingPeriodicity2", StringType(), True)) \
        .add(StructField("MeteringMethod", StringType(), True)) \
        .add(StructField("MeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("ConnectionState", StringType(), True)) \
        .add(StructField("EnergySupplier_MarketParticipant_mRID", StringType(), True)) \
        .add(StructField("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), True)) \
        .add(StructField("InMeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("OutMeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("Parent_Domain", StringType(), True)) \
        .add(StructField("SupplierAssociationId", StringType(), True)) \
        .add(StructField("ServiceCategoryKind", StringType(), True)) \
        .add(StructField("MarketEvaluationPointType", StringType(), True)) \
        .add(StructField("SettlementMethod", StringType(), True)) \
        .add(StructField("UnitName", StringType(), True)) \
        .add(StructField("Product", StringType(), True)) \
        .add(StructField("VR-245-1-Is-Valid", BooleanType(), True)) \
        .add(StructField("VR-250-Is-Valid", BooleanType(), True)) \
        .add(StructField("VR-251-Is-Valid", BooleanType(), True)) \
        .add(StructField("VR-611-Is-Valid", BooleanType(), True)) \
        .add(StructField("VR-612-Is-Valid", BooleanType(), True))


# Mock parsed, master dataframes
@pytest.fixture(scope="module")
def parsed_data_pandas_df():
    return pd.DataFrame({
        "MarketEvaluationPoint_mRID": ["1", "2", "1", "1"],
        "ObservationTime": [timestamp_now, timestamp_now, timestamp_now, timestamp_now],
        "Quantity": [1.0, 2.0, 3.0, -4.0],
        "CorrelationId": ["a", "a", "a", "a"],
        "MessageReference": ["b", "b", "b", "b"],
        "HeaderEnergyDocument_mRID": ["c", "c", "c", "c"],
        "HeaderEnergyDocumentCreation": [timestamp_now, timestamp_now, timestamp_now, timestamp_now],
        "HeaderEnergyDocumentSenderIdentification": ["d", "d", "d", "d"],
        "EnergyBusinessProcess": ["e", "e", "e", "e"],
        "EnergyBusinessProcessRole": ["f", "f", "f", "f"],
        "TimeSeriesmRID": ["g", "g", "g", "g"],
        "MktActivityRecord_Status": ["h", "h", "h", "h"],
        "Product": ["i", "i", "i", "i"],
        "UnitName": ["j", "j", "j", "j"],
        "MarketEvaluationPointType": ["EPTypeA", "EPTypeA", "EPTypeB", "EPTypeA"],
        "Quality": ["l", "l", "l", "l"],
        "EventHubEnqueueTime": [timestamp_now, timestamp_now, timestamp_now, timestamp_now]})


@pytest.fixture(scope="module")
def master_data_pandas_df():
    return pd.DataFrame({
        "MarketEvaluationPoint_mRID": ["1"],
        "MeterReadingPeriodicity": ["a"],
        "MeteringMethod": ["b"],
        "MeterReadingPeriodicity2": ["c"],
        "MeteringGridArea_Domain_mRID": ["d"],
        "ConnectionState": ["e"],
        "EnergySupplier_MarketParticipant_mRID": ["f"],
        "BalanceResponsibleParty_MarketParticipant_mRID": ["g"],
        "InMeteringGridArea_Domain_mRID": ["h"],
        "OutMeteringGridArea_Domain_mRID": ["i"],
        "Parent_Domain": ["j"],
        "SupplierAssociationId": ["k"],
        "ServiceCategoryKind": ["l"],
        "MarketEvaluationPointType": ["EPTypeA"],
        "SettlementMethod": ["n"],
        "UnitName": ["o"],
        "Product": ["p"]})


# "Enriches" data by joining parsed and master dataframes - necessary to get proper column aliasing
def mock_enrich(parsed_data, master_data):
    return parsed_data.alias("pd") \
        .join(master_data.alias("md"),
              (col("pd.MarketEvaluationPoint_mRID") == col("md.MarketEvaluationPoint_mRID")), how="left") \
        .drop(master_data["MarketEvaluationPoint_mRID"])


@pytest.fixture(scope="module")
def validated_data_df(spark, parsed_data_pandas_df, master_data_pandas_df, parsed_data_schema, master_data_schema):
    parsed_data = spark.createDataFrame(parsed_data_pandas_df, schema=parsed_data_schema)
    master_data = spark.createDataFrame(master_data_pandas_df, schema=master_data_schema)
    enriched_data = mock_enrich(parsed_data, master_data)
    return Validator.validate(enriched_data)


# Test 5: check to see if validate function returns DataFrame with correct schema
def test_validate_returns_proper_schema(validated_data_df, expected_validate_schema):
    assert validated_data_df.schema == expected_validate_schema
