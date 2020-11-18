from datetime import datetime, timezone
import pandas as pd
from geh_stream.aggregation_utils.filters import TimePeriodFilter
from pyspark.sql import SparkSession
from pyspark.sql.types import StructType, TimestampType, IntegerType
import pytest

date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"

# Create the utc and pst versions of datetimes used to filter to check how the filtration works with different timezones
to_time_utc = datetime.strptime("2020-01-01T00:00:00+0000", date_time_formatting_string)
from_time_utc = datetime.strptime("2019-01-01T00:00:00+0000", date_time_formatting_string)
to_time_pst = datetime.strptime("2020-01-01T00:00:00-0800", date_time_formatting_string)
from_time_pst = datetime.strptime("2019-01-01T00:00:00-0800", date_time_formatting_string)


@pytest.fixture(scope="module")
def observation_schema():
    """Schema of the test DataFrame"""
    return StructType() \
        .add("ObservationTime", TimestampType(), False) \
        .add("id", IntegerType(), False)


@pytest.fixture(scope="module")
def observation_data_frame(spark, observation_schema):
    """
    Create the observation data using UTC datetimes.
    Here we want to make sure correct rows remain after the filtering
    We use the "id" column for these checks rather than the datetimes because interestingly, when we
    grab a datetime from the dataframe, it is timezone naive so we cannot do comparisons to the input data.
        From https://stackoverflow.com/questions/48746376/timestamptype-in-pyspark-with-datetime-tzaware-objects:
        "TimestampType in pyspark is not tz aware like in Pandas rather it passes long ints
        and displays them according to your machine's local time zone (by default)"
    """
    pandas_df = pd.DataFrame({
        'ObservationTime': [
            datetime.strptime("2018-01-01T00:00:00+0000", date_time_formatting_string),
            datetime.strptime("2019-01-01T00:00:00+0000", date_time_formatting_string),
            datetime.strptime("2019-02-01T00:00:00+0000", date_time_formatting_string),
            datetime.strptime("2020-01-01T00:00:00+0000", date_time_formatting_string),
            datetime.strptime("2021-01-01T00:00:00+0000", date_time_formatting_string)
        ],
        "id": [1, 2, 3, 4, 5]})
    return spark.createDataFrame(pandas_df, schema=observation_schema)


@pytest.fixture(scope="module")
def time_period_utc_filtered_data_frame(observation_data_frame):
    return TimePeriodFilter.filter(observation_data_frame, from_time_utc, to_time_utc)


@pytest.fixture(scope="module")
def time_period_pst_filtered_data_frame(observation_data_frame):
    return TimePeriodFilter.filter(observation_data_frame, from_time_pst, to_time_pst)


def test_time_period_utc_filter_filters_out_rows_not_in_range(time_period_utc_filtered_data_frame):
    assert time_period_utc_filtered_data_frame.count() == 3
    assert time_period_utc_filtered_data_frame.toPandas()["id"][0] == 2
    assert time_period_utc_filtered_data_frame.toPandas()["id"][1] == 3
    assert time_period_utc_filtered_data_frame.toPandas()["id"][2] == 4


def test_time_period_pst_filter_filters_out_rows_not_in_range(time_period_pst_filtered_data_frame):
    """Test to see whether time period filter still works if time period has different tz than data itself"""
    assert time_period_pst_filtered_data_frame.count() == 2
    assert time_period_pst_filtered_data_frame.toPandas()["id"][0] == 3
    assert time_period_pst_filtered_data_frame.toPandas()["id"][1] == 4
