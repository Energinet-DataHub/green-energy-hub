import pytest
import pandas as pd
import time
from geh_stream.streaming_utils import Enricher
from geh_stream.schemas import SchemaNames, SchemaFactory
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import DoubleType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col

# Create timestamps used in DataFrames
time_now = time.time()
time_future = time.time() + 1000
time_past = time.time() - 1000
time_far_future = time.time() + 10000
timestamp_now = pd.Timestamp(time_now, unit='s')
timestamp_future = pd.Timestamp(time_future, unit='s')
timestamp_past = pd.Timestamp(time_past, unit='s')
timestamp_far_future = pd.Timestamp(time_far_future, unit='s')


# Create schema of parsed data (timeseries data) and master data
@pytest.fixture(scope="class")
def parsed_schema():
    return SchemaFactory.get_instance(SchemaNames.Parsed)


@pytest.fixture(scope="class")
def master_schema():
    return SchemaFactory.get_instance(SchemaNames.Master)


# Create parsed data and master data Dataframes
@pytest.fixture(scope="class")
def master_data(spark, master_schema):
    pandas_df = pd.DataFrame({
        'MarketEvaluationPoint_mRID': ["1"],
        "ValidFrom": [timestamp_past],
        "ValidTo": [timestamp_future],
        "MeterReadingPeriodicity": ["a"],
        "MeteringMethod": ["b"],
        "MeteringGridArea_Domain_mRID": ["d"],
        "ConnectionState": ["e"],
        "EnergySupplier_MarketParticipant_mRID": ["f"],
        "BalanceResponsibleParty_MarketParticipant_mRID": ["g"],
        "InMeteringGridArea_Domain_mRID": ["h"],
        "OutMeteringGridArea_Domain_mRID": ["i"],
        "Parent_Domain_mRID": ["j"],
        "ServiceCategory_Kind": ["l"],
        "MarketEvaluationPointType": ["m"],
        "SettlementMethod": ["n"],
        "QuantityMeasurementUnit_Name": ["o"],
        "Product": ["p"],
        "Technology": ["t"]})
    return spark.createDataFrame(pandas_df, schema=master_schema)


@pytest.fixture(scope="class")
def parsed_data(spark, parsed_schema):
    pandas_df = pd.DataFrame({
        "MarketEvaluationPoint_mRID": ["1", "1", "2"],
        "ObservationTime": [timestamp_now, timestamp_far_future, timestamp_now],
        "Quantity": [1.0, 2.0, 3.0],
        "CorrelationId": ["a", "aa", "aaa"],
        "MessageReference": ["b", "b", "b"],
        "MarketDocument_mRID": ["c", "c", "c"],
        "CreatedDateTime": [timestamp_now, timestamp_now, timestamp_now],
        "SenderMarketParticipant_mRID": ["d", "d", "d"],
        "ProcessType": ["e", "e", "e"],
        "SenderMarketParticipantMarketRole_Type": ["f", "f", "f"],
        "TimeSeries_mRID": ["g", "g", "g"],
        "MktActivityRecord_Status": ["h", "h", "h"],
        "Product": ["i", "i", "i"],
        "QuantityMeasurementUnit_Name": ["j", "j", "j"],
        "MarketEvaluationPointType": ["k", "k", "k"],
        "Quality": ["l", "l", "l"],
        "EventHubEnqueueTime": [timestamp_now, timestamp_now, timestamp_now]})
    return spark.createDataFrame(pandas_df, schema=parsed_schema)


# Run the enrich function
@pytest.fixture(scope="class")
def enriched_data(parsed_data, master_data):
    return Enricher.enrich(parsed_data, master_data)


# Create the expected schema
# We cannot simply combine the two schemas (master and parsed) because due to the join,
# all fields from master data becomes nullable and some columns are dropped.
@pytest.fixture(scope="class")
def expected_schema():
    return StructType() \
        .add(StructField("MarketEvaluationPoint_mRID", StringType(), False)) \
        .add(StructField("ObservationTime", TimestampType(), False)) \
        .add(StructField("Quantity", DoubleType(), True)) \
        .add(StructField("CorrelationId", StringType(), True)) \
        .add(StructField("MessageReference", StringType(), True)) \
        .add(StructField("MarketDocument_mRID", StringType(), True)) \
        .add(StructField("CreatedDateTime", TimestampType(), True)) \
        .add(StructField("SenderMarketParticipant_mRID", StringType(), True)) \
        .add(StructField("ProcessType", StringType(), True)) \
        .add(StructField("SenderMarketParticipantMarketRole_Type", StringType(), True)) \
        .add(StructField("TimeSeries_mRID", StringType(), True)) \
        .add(StructField("MktActivityRecord_Status", StringType(), True)) \
        .add(StructField("Product", StringType(), True)) \
        .add(StructField("QuantityMeasurementUnit_Name", StringType(), True)) \
        .add(StructField("MarketEvaluationPointType", StringType(), True)) \
        .add(StructField("Quality", StringType(), True)) \
        .add(StructField("EventHubEnqueueTime", TimestampType(), False)) \
        .add(StructField("MeterReadingPeriodicity", StringType(), True)) \
        .add(StructField("MeteringMethod", StringType(), True)) \
        .add(StructField("MeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("ConnectionState", StringType(), True)) \
        .add(StructField("EnergySupplier_MarketParticipant_mRID", StringType(), True)) \
        .add(StructField("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), True)) \
        .add(StructField("InMeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("OutMeteringGridArea_Domain_mRID", StringType(), True)) \
        .add(StructField("Parent_Domain_mRID", StringType(), True)) \
        .add(StructField("ServiceCategory_Kind", StringType(), True)) \
        .add(StructField("MarketEvaluationPointType", StringType(), True)) \
        .add(StructField("SettlementMethod", StringType(), True)) \
        .add(StructField("QuantityMeasurementUnit_Name", StringType(), True)) \
        .add(StructField("Product", StringType(), True)) \
        .add(StructField("Technology", StringType(), True))


# Is the row count maintained
def test_enrich_returns_correct_row_count(enriched_data):
    assert enriched_data.count() == 3


# Does the join work correctly given the sample data
def test_enrich_joins_matching_parsed_row_with_master_data(enriched_data):
    matched_rows = enriched_data.filter(col("md.ConnectionState").isNotNull())
    assert matched_rows.count() == 1
    assert matched_rows.first().CorrelationId == "a"


# Are there 2 rows left with null master data fields (for the rows that don't fit the join conditions)
def test_enrich_keeps_unmatched_rows(enriched_data):
    unmatched_rows = enriched_data.filter(col("md.ConnectionState").isNull())
    assert unmatched_rows.count() == 2


# Is the schema of the returned DataFrame as expected
def test_enrich_returns_proper_schema(enriched_data, expected_schema):
    assert enriched_data.schema == expected_schema
