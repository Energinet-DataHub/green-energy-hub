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
from datetime import datetime, timedelta
from enum import Enum
from geh_stream.aggregation_utils.aggregators import GridLossCalculator
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import StructType, StringType, DecimalType, TimestampType
from pyspark.sql.functions import col
import pytest
import pandas as pd


date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"
default_obs_time = datetime.strptime("2020-01-01T00:00:00+0000", date_time_formatting_string)


class AggregationMethod(Enum):
    net_exchange = 'net_exchange'
    hourly_consumption = 'hourly_consumption'
    flex_consumption = 'flex_consumption'
    production = 'production'


@pytest.fixture(scope="module")
def agg_net_exchange_schema():
    return StructType() \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("time_window",
             StructType()
             .add("start", TimestampType())
             .add("end", TimestampType())
             ) \
        .add("in_sum", DecimalType(38)) \
        .add("out_sum", DecimalType(38)) \
        .add("result", DecimalType(38))


@pytest.fixture(scope="module")
def agg_consumption_and_production_schema():
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
def agg_result_factory(spark, agg_net_exchange_schema, agg_consumption_and_production_schema):
    """
    Factory to generate a single row of time series data, with default parameters as specified above.
    """
    def factory(agg_method: AggregationMethod):
        if agg_method == AggregationMethod.net_exchange:
            pandas_df = pd.DataFrame({
                'MeteringGridArea_Domain_mRID': [],
                'time_window': [],
                'in_sum': [],
                'out_sum': [],
                'result': [],
            })
            for i in range(10):
                pandas_df = pandas_df.append({
                    'MeteringGridArea_Domain_mRID': str(i),
                    'time_window': default_obs_time + timedelta(hours=i),
                    'in_sum': Decimal(1),
                    'out_sum': Decimal(1),
                    'result': Decimal(20 + i),
                }, ignore_index=True)
            return spark.createDataFrame(pandas_df, schema=agg_net_exchange_schema)
        elif agg_method == AggregationMethod.hourly_consumption:
            pandas_df = pd.DataFrame({
                'MeteringGridArea_Domain_mRID': [],
                'BalanceResponsibleParty_MarketParticipant_mRID': [],
                'EnergySupplier_MarketParticipant_mRID': [],
                'time_window': [],
                'sum_quantity': [],
            })
            for i in range(10):
                pandas_df = pandas_df.append({
                    'MeteringGridArea_Domain_mRID': str(i),
                    'BalanceResponsibleParty_MarketParticipant_mRID': str(i),
                    'EnergySupplier_MarketParticipant_mRID': str(i),
                    'time_window': default_obs_time + timedelta(hours=i),
                    'sum_quantity': Decimal(13 + i),
                }, ignore_index=True)
            return spark.createDataFrame(pandas_df, schema=agg_consumption_and_production_schema)
        elif agg_method == AggregationMethod.flex_consumption:
            pandas_df = pd.DataFrame({
                'MeteringGridArea_Domain_mRID': [],
                'BalanceResponsibleParty_MarketParticipant_mRID': [],
                'EnergySupplier_MarketParticipant_mRID': [],
                'time_window': [],
                'sum_quantity': [],
            })
            for i in range(10):
                pandas_df = pandas_df.append({
                    'MeteringGridArea_Domain_mRID': str(i),
                    'BalanceResponsibleParty_MarketParticipant_mRID': str(i),
                    'EnergySupplier_MarketParticipant_mRID': str(i),
                    'time_window': default_obs_time + timedelta(hours=i),
                    'sum_quantity': Decimal(14 + i),
                }, ignore_index=True)
            return spark.createDataFrame(pandas_df, schema=agg_consumption_and_production_schema)
        elif agg_method == AggregationMethod.production:
            pandas_df = pd.DataFrame({
                'MeteringGridArea_Domain_mRID': [],
                'BalanceResponsibleParty_MarketParticipant_mRID': [],
                'EnergySupplier_MarketParticipant_mRID': [],
                'time_window': [],
                'sum_quantity': [],
            })
            for i in range(10):
                pandas_df = pandas_df.append({
                    'MeteringGridArea_Domain_mRID': str(i),
                    'BalanceResponsibleParty_MarketParticipant_mRID': str(i),
                    'EnergySupplier_MarketParticipant_mRID': str(i),
                    'time_window': default_obs_time + timedelta(hours=i),
                    'sum_quantity': Decimal(50 + i),
                }, ignore_index=True)
            return spark.createDataFrame(pandas_df, schema=agg_consumption_and_production_schema)
    return factory


def test_grid_loss_calculation(agg_result_factory):
    agg_net_exchange = agg_result_factory(agg_method=AggregationMethod.net_exchange)
    agg_hourly_consumption = agg_result_factory(agg_method=AggregationMethod.hourly_consumption)
    agg_flex_consumption = agg_result_factory(agg_method=AggregationMethod.flex_consumption)
    agg_production = agg_result_factory(agg_method=AggregationMethod.production)

    result = GridLossCalculator.calculate(
        agg_net_exchange=agg_net_exchange,
        agg_hourly_consumption=agg_hourly_consumption,
        agg_flex_consumption=agg_flex_consumption,
        agg_production=agg_production)

    # Verify the calculation result is correct by checking 50+i + 20+i - (13+i + 14+i) equals 43 for all i in range 0 to 9
    assert result.filter(col('grid_loss') != 43).count() == 0
