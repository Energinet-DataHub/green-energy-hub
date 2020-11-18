import copy
import pytest
import pandas as pd
import time
from geh_stream.streaming_utils import EventHubParser
from geh_stream.schemas import SchemaNames, SchemaFactory
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import BinaryType, BooleanType, DoubleType, LongType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col, lit

# Create timestamp used in DataFrames
time_now = time.time()
timestamp_now = pd.Timestamp(time_now, unit='s')


# Schemas of Event Hub Message, nested json body message, and expected result Dataframe from parse function
@pytest.fixture(scope="class")
def event_hub_message_schema():
    return StructType() \
        .add("body", BinaryType(), False) \
        .add("partition", StringType(), False) \
        .add("offset", StringType(), False) \
        .add("sequenceNumber", LongType(), False) \
        .add("enqueuedTime", StringType(), False) \
        .add("publisher", StringType(), False) \
        .add("partitionKey", StringType(), False) \
        .add("properties", StructType(), True) \
        .add("systemProperties", StructType(), True)


@pytest.fixture(scope="class")
def json_body_message_schema():
    return SchemaFactory.get_instance(SchemaNames.MessageBody)


# NOTE: MarketEvaluationPoint_mRID, and ObservationTime should be non nullable but it looks
#       like whenever one uses the from_json function, everyting becomes non nullable.
@pytest.fixture(scope="class")
def expected_parsed_data_schema():
    return StructType() \
        .add("MarketEvaluationPoint_mRID", StringType(), True) \
        .add("ObservationTime", TimestampType(), True) \
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
        .add("Quality", StringType(), True) \
        .add("EventHubEnqueueTime", StringType(), False)


@pytest.fixture(scope="class")
def event_hub_message_df(event_hub_message_schema, spark):
    # Create message body using the required fields
    body_message = "{\"MarketEvaluationPoint_mRID\":\"1\", \"ObservationTime\":\"" + str(timestamp_now) + "\"}"
    binary_body_message = bytes(body_message, encoding="utf8")

    # Create event hub message
    event_hub_message_pandas_df = pd.DataFrame({
        "body": [binary_body_message],
        "partition": ["1"],
        "offset": ["offset"],
        "sequenceNumber": [2],
        "enqueuedTime": [timestamp_now],
        "publisher": ["publisher"],
        "partitionKey": ["partitionKey"],
        "properties": [None],
        "systemProperties": [None], })

    return spark.createDataFrame(event_hub_message_pandas_df, event_hub_message_schema)


@pytest.fixture(scope="class")
def parsed_data(event_hub_message_df, json_body_message_schema):
    return EventHubParser.parse(event_hub_message_df, json_body_message_schema)


# Check that the nested json is parsed correctly
def test_parse_event_hub_message_returns_correct_nested_columns(parsed_data):
    assert parsed_data.first().MarketEvaluationPoint_mRID == "1"


# Check that resulting DataFrame has expected schema *See NOTE above
def test_parse_event_hub_message_returns_correct_schema(parsed_data, expected_parsed_data_schema):
    assert parsed_data.schema == expected_parsed_data_schema
