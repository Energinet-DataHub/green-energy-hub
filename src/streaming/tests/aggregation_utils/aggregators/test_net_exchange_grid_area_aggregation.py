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
import pytest
from decimal import Decimal
import pandas as pd
from datetime import datetime, timedelta
from geh_stream.aggregation_utils.aggregators import NetExchangeGridAreaAggregator
from geh_stream.codelists import MarketEvaluationPointType
from pyspark.sql import DataFrame
import pyspark.sql.functions as F
from pyspark.sql.types import StructType, StringType, DecimalType, TimestampType


e_20 = MarketEvaluationPointType.exchange.value
date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"
default_obs_time = datetime.strptime(
    "2020-01-01T00:00:00+0000", date_time_formatting_string)
numberOfTestHours = 24

# Time series schema


@pytest.fixture(scope="module")
def time_series_schema():
    return StructType() \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("InMeteringGridArea_Domain_mRID", StringType()) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), False) \
        .add("Quantity", DecimalType(38, 10)) \
        .add("ObservationTime", TimestampType())


@pytest.fixture(scope="module")
def expected_schema():
    """
    Expected exchange aggregation output schema
    """
    return StructType() \
        .add("grid_id", StringType()) \
        .add("time_window",
             StructType()
             .add("start", TimestampType())
             .add("end", TimestampType())
             ) \
        .add("in_sum", DecimalType(38, 10)) \
        .add("out_sum", DecimalType(38, 10)) \
        .add("result", DecimalType(38, 9))


@pytest.fixture(scope="module")
def time_series_data_frame(spark, time_series_schema):
    """
    Sample Time Series DataFrame
    """
    # Create empty pandas df
    pandas_df = pd.DataFrame({
        'MarketEvaluationPointType': [],
        'InMeteringGridArea_Domain_mRID': [],
        "OutMeteringGridArea_Domain_mRID": [],
        "Quantity": [],
        "ObservationTime": []
    })

    # add 24 hours of exchange with different examples of exchange between grid areas. See readme.md for more info

    for x in range(numberOfTestHours):
        pandas_df = add_row_of_data(pandas_df, e_20, "B", "A", Decimal(2) * x, default_obs_time + timedelta(hours=x))

        pandas_df = add_row_of_data(pandas_df, e_20, "B", "A", Decimal('0.5') * x, default_obs_time + timedelta(hours=x))
        pandas_df = add_row_of_data(pandas_df, e_20, "B", "A", Decimal('0.7') * x, default_obs_time + timedelta(hours=x))

        pandas_df = add_row_of_data(pandas_df, e_20, "A", "B", Decimal(3) * x, default_obs_time + timedelta(hours=x))
        pandas_df = add_row_of_data(pandas_df, e_20, "A", "B", Decimal('0.9') * x, default_obs_time + timedelta(hours=x))
        pandas_df = add_row_of_data(pandas_df, e_20, "A", "B", Decimal('1.2') * x, default_obs_time + timedelta(hours=x))

        pandas_df = add_row_of_data(pandas_df, e_20, "C", "A", Decimal('0.7') * x, default_obs_time + timedelta(hours=x))
        pandas_df = add_row_of_data(pandas_df, e_20, "A", "C", Decimal('1.1') * x, default_obs_time + timedelta(hours=x))
        pandas_df = add_row_of_data(pandas_df, e_20, "A", "C", Decimal('1.5') * x, default_obs_time + timedelta(hours=x))

    return spark.createDataFrame(pandas_df, schema=time_series_schema)


def add_row_of_data(pandas_df: pd.DataFrame, point_type, in_domain, out_domain, quantity: Decimal, timestamp):
    """
    Helper method to create a new row in the dataframe to improve readability and maintainability
    """
    new_row = {
        "MarketEvaluationPointType": point_type,
        "InMeteringGridArea_Domain_mRID": in_domain,
        "OutMeteringGridArea_Domain_mRID": out_domain,
        "Quantity": quantity,
        "ObservationTime": timestamp
    }
    return pandas_df.append(new_row, ignore_index=True)


@pytest.fixture(scope="module")
def aggregated_data_frame(time_series_data_frame):
    """Perform aggregation"""
    return NetExchangeGridAreaAggregator.aggregate(time_series_data_frame)


def test_test_data_has_correct_row_count(time_series_data_frame):
    """ Check sample data row count"""
    assert time_series_data_frame.count() == (9 * numberOfTestHours)


def test_exchange_aggregator_returns_correct_schema(aggregated_data_frame, expected_schema):
    """Check aggregation schema"""
    assert aggregated_data_frame.schema == expected_schema


def test_exchange_aggregator_returns_correct_aggregations(aggregated_data_frame):
    """Check accuracy of aggregations"""

    for x in range(numberOfTestHours):
        check_aggregation_row(aggregated_data_frame, "A", Decimal('3.8') * x, default_obs_time + timedelta(hours=x))
        check_aggregation_row(aggregated_data_frame, "B", Decimal('-1.9') * x, default_obs_time + timedelta(hours=x))
        check_aggregation_row(aggregated_data_frame, "C", Decimal('-1.9') * x, default_obs_time + timedelta(hours=x))


def check_aggregation_row(df: DataFrame, grid_id: str, sum: Decimal, time: datetime):
    """Helper function that checks column values for the given row"""
    gridfiltered = df.filter(df["grid_id"] == grid_id).select(F.col("grid_id"), F.col(
        "result"), F.col("time_window.start").alias("start"), F.col("time_window.end").alias("end"))
    res = gridfiltered.filter(gridfiltered["start"] == time).toPandas()
    assert res["result"][0] == sum
