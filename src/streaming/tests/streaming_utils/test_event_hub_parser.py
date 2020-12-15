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
import pytest
import pandas as pd
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import BinaryType, LongType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col, lit
import time

from geh_stream.streaming_utils import EventHubParser
from geh_stream.schemas import SchemaNames, SchemaFactory

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
        .add("publisher", StringType(), False) \
        .add("partitionKey", StringType(), False) \
        .add("properties", StructType(), True) \
        .add("systemProperties", StructType(), True)


@pytest.fixture(scope="class")
def json_body_message_schema():
    return SchemaFactory.get_instance(SchemaNames.MessageBody)


@pytest.fixture(scope="class")
def event_hub_message_df(event_hub_message_schema, time_series_json, spark):
    # Create message body using the required fields
    binary_body_message = bytes(time_series_json, encoding="utf8")

    # Create event hub message
    event_hub_message_pandas_df = pd.DataFrame({
        "body": [binary_body_message],
        "partition": ["1"],
        "offset": ["offset"],
        "sequenceNumber": [2],
        "publisher": ["publisher"],
        "partitionKey": ["partitionKey"],
        "properties": [None],
        "systemProperties": [None], })

    return spark.createDataFrame(event_hub_message_pandas_df, event_hub_message_schema)


@pytest.fixture(scope="class")
def parsed_data(event_hub_message_df, json_body_message_schema):
    return EventHubParser.parse(event_hub_message_df, json_body_message_schema)


# Check that the nested json is parsed correctly
def test_parse_event_hub_message_returns_correct_nested_columns(parsed_data_factory):
    market_evaluation_point_mrid = "mrid123"
    parsed_data = parsed_data_factory(dict(market_evaluation_point_mrid=market_evaluation_point_mrid))
    print(parsed_data.first())
    assert parsed_data.first().MarketEvaluationPoint_mRID == market_evaluation_point_mrid


# Check that resulting DataFrame has expected schema
def test_parse_event_hub_message_returns_correct_schema(parsed_data):
    assert str(parsed_data.schema) == str(SchemaFactory.get_instance(SchemaNames.Parsed))
