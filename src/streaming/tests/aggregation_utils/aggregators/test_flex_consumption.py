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
from decimal import Decimal
from datetime import datetime
from geh_stream.aggregation_utils.aggregators import FlexConsumptionAggregator
from geh_stream.codelists import MarketEvaluationPointType, SettlementMethod
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import StructType, StringType, DecimalType, TimestampType
import pytest
import pandas as pd

e_20 = MarketEvaluationPointType.exchange.value
e_17 = MarketEvaluationPointType.consumption.value
e_18 = MarketEvaluationPointType.production.value
e_01 = SettlementMethod.profiled.value
e_02 = SettlementMethod.non_profiled.value
d_01 = SettlementMethod.flex_settled.value

# Default time series data point values
default_point_type = e_17
default_settlement_method = d_01
default_domain = "D1"
default_responsible = "R1"
default_supplier = "S1"
default_quantity = Decimal(1)

date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"
default_obs_time = datetime.strptime("2020-01-01T00:00:00+0000", date_time_formatting_string)


@pytest.fixture(scope="module")
def time_series_schema():
    """
    Input time series data point schema
    """
    return StructType() \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType()) \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType()) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType()) \
        .add("Quantity", DecimalType()) \
        .add("ObservationTime", TimestampType())


@pytest.fixture(scope="module")
def expected_schema():
    """
    Expected aggregation schema
    NOTE: Spark seems to add 10 to the precision of the decimal type on summations.
    Thus, the expected schema should be precision of 20, 10 more than the default of 10.
    If this is an issue we can always cast back to the original decimal precision in the aggregate
    function.
    https://stackoverflow.com/questions/57203383/spark-sum-and-decimaltype-precision
    """
    return StructType() \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType()) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType()) \
        .add("time_window",
             StructType()
             .add("start", TimestampType())
             .add("end", TimestampType()),
             False) \
        .add("sum_quantity", DecimalType(20))


@pytest.fixture(scope="module")
def time_series_row_factory(spark, time_series_schema):
    """
    Factory to generate a single row of time series data, with default parameters as specified above.
    """
    def factory(point_type=default_point_type,
                settlement_method=default_settlement_method,
                domain=default_domain,
                responsible=default_responsible,
                supplier=default_supplier,
                quantity=default_quantity,
                obs_time=default_obs_time):
        pandas_df = pd.DataFrame({
            "MarketEvaluationPointType": [point_type],
            "SettlementMethod": [settlement_method],
            "MeteringGridArea_Domain_mRID": [domain],
            "BalanceResponsibleParty_MarketParticipant_mRID": [responsible],
            "EnergySupplier_MarketParticipant_mRID": [supplier],
            "Quantity": [quantity],
            "ObservationTime": [obs_time]})
        return spark.createDataFrame(pandas_df, schema=time_series_schema)
    return factory


def check_aggregation_row(df: DataFrame, row: int, grid: str, responsible: str, supplier: str, sum: Decimal, start: datetime, end: datetime):
    """
    Helper function that checks column values for the given row.
    Note that start and end datetimes are timezone-naive - we set the Spark session timezone to UTC in the
    conftest.py, since:
        From https://stackoverflow.com/questions/48746376/timestamptype-in-pyspark-with-datetime-tzaware-objects:
        "TimestampType in pyspark is not tz aware like in Pandas rather it passes long ints
        and displays them according to your machine's local time zone (by default)"
    """
    pandas_df = df.toPandas()
    assert pandas_df["MeteringGridArea_Domain_mRID"][row] == grid
    assert pandas_df["BalanceResponsibleParty_MarketParticipant_mRID"][row] == responsible
    assert pandas_df["EnergySupplier_MarketParticipant_mRID"][row] == supplier
    assert pandas_df["sum_quantity"][row] == sum
    assert pandas_df["time_window"][row].start == start
    assert pandas_df["time_window"][row].end == end


@pytest.mark.parametrize(
    "point_type",
    [
        pytest.param(
            e_18, id="should filter out E18"
        ),
        pytest.param(
            e_20, id="should filter out E20"
        )
    ],
)
def test_filters_out_incorrect_point_type(point_type, time_series_row_factory):
    """
    Aggregator should filter out all non "E17" MarketEvaluationPointType rows
    """
    df = time_series_row_factory(point_type=point_type)
    aggregated_df = FlexConsumptionAggregator.aggregate(df)
    assert aggregated_df.count() == 0


@pytest.mark.parametrize(
    "settlement_method",
    [
        pytest.param(
            e_01, id="should filter out E01"
        ),
        pytest.param(
            e_02, id="should filter out E02"
        )
    ],
)
def test_filters_out_incorrect_settlement_method(settlement_method, time_series_row_factory):
    """
    Aggregator should filter out all non "D01" SettlementMethod rows
    """
    df = time_series_row_factory(settlement_method=settlement_method)
    aggregated_df = FlexConsumptionAggregator.aggregate(df)
    assert aggregated_df.count() == 0


def test_aggregates_observations_in_same_hour(time_series_row_factory):
    """
    Aggregator should can calculate the correct sum of a "domain"-"responsible"-"supplier" grouping within the
    same 1hr time window
    """
    row1_df = time_series_row_factory(quantity=Decimal(1))
    row2_df = time_series_row_factory(quantity=Decimal(2))
    df = row1_df.union(row2_df)
    aggregated_df = FlexConsumptionAggregator.aggregate(df)

    # Create the start/end datetimes representing the start and end of the 1 hr time period
    # These should be datetime naive in order to compare to the Spark Dataframe
    start_time = datetime(2020, 1, 1, 0, 0, 0)
    end_time = datetime(2020, 1, 1, 1, 0, 0)

    assert aggregated_df.count() == 1
    check_aggregation_row(aggregated_df, 0, default_domain, default_responsible, default_supplier, Decimal(3), start_time, end_time)


def test_returns_distinct_rows_for_observations_in_different_hours(time_series_row_factory):
    """
    Aggregator should can calculate the correct sum of a "domain"-"responsible"-"supplier" grouping within the
    2 different 1hr time windows
    """
    diff_obs_time = datetime.strptime("2020-01-01T01:00:00+0000", date_time_formatting_string)

    row1_df = time_series_row_factory()
    row2_df = time_series_row_factory(obs_time=diff_obs_time)
    df = row1_df.union(row2_df)
    aggregated_df = FlexConsumptionAggregator.aggregate(df)

    assert aggregated_df.count() == 2

    # Create the start/end datetimes representing the start and end of the 1 hr time period for each row's ObservationTime
    # These should be datetime naive in order to compare to the Spark Dataframe
    start_time_row1 = datetime(2020, 1, 1, 0, 0, 0)
    end_time_row1 = datetime(2020, 1, 1, 1, 0, 0)
    check_aggregation_row(aggregated_df, 0, default_domain, default_responsible, default_supplier, default_quantity, start_time_row1, end_time_row1)

    start_time_row2 = datetime(2020, 1, 1, 1, 0, 0)
    end_time_row2 = datetime(2020, 1, 1, 2, 0, 0)
    check_aggregation_row(aggregated_df, 1, default_domain, default_responsible, default_supplier, default_quantity, start_time_row2, end_time_row2)


def test_returns_correct_schema(time_series_row_factory, expected_schema):
    """
    Aggregator should return the correct schema, including the proper fields for the aggregated quantity values
    and time window (from the single-hour resolution specified in the aggregator).
    """
    df = time_series_row_factory()
    aggregated_df = FlexConsumptionAggregator.aggregate(df)
    assert aggregated_df.schema == expected_schema
