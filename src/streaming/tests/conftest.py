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
"""
By having a conftest.py in this directory, we are able to add all packages
defined in the geh_stream directory in our tests.
"""

import pytest
from pyspark import SparkConf
from pyspark.sql import SparkSession, DataFrame
import pandas as pd
import time
from pyspark.sql.types import DoubleType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col, lit
from geh_stream.codelists import MarketEvaluationPointType, Quality
from geh_stream.streaming_utils.enrichers.master_data_enricher import enrich_master_data
from geh_stream.schemas import SchemaNames, SchemaFactory


# Create Spark Conf/Session
@pytest.fixture(scope="session")
def spark():
    spark_conf = SparkConf(loadDefaults=True) \
        .set("spark.sql.session.timeZone", "UTC")
    return SparkSession \
        .builder \
        .config(conf=spark_conf) \
        .getOrCreate()


# Create timestamps used in DataFrames
time_now = time.time()
time_future = time.time() + 1000
time_past = time.time() - 1000
timestamp_now = pd.Timestamp(time_now, unit='s')
timestamp_future = pd.Timestamp(time_future, unit='s')
timestamp_past = pd.Timestamp(time_past, unit='s')


# Create schema of parsed data (timeseries data) and master data
@pytest.fixture(scope="session")
def parsed_schema():
    return SchemaFactory.get_instance(SchemaNames.Parsed)


@pytest.fixture(scope="session")
def master_schema():
    return SchemaFactory.get_instance(SchemaNames.Master)


# Create parsed data and master data Dataframes
@pytest.fixture(scope="session")
def master_data_factory(spark, master_schema):
    def factory(market_evaluation_point_type,
                marketparticipant_mrid,
                meteringgridarea_domain_mrid,
                inmeteringgridarea_domain_mrid,
                inmeteringgridownerarea_domain_mrid,
                outmeteringgridarea_domain_mrid,
                outmeteringgridownerarea_domain_mrid,
                settlement_method):
        pandas_df = pd.DataFrame({
            'MarketEvaluationPoint_mRID': ["1"],
            "ValidFrom": [timestamp_past],
            "ValidTo": [timestamp_future],
            "MeterReadingPeriodicity": ["a"],
            "MeteringMethod": ["b"],
            "MeteringGridArea_Domain_mRID": [meteringgridarea_domain_mrid],
            "ConnectionState": ["e"],
            "EnergySupplier_MarketParticipant_mRID": [marketparticipant_mrid],
            "BalanceResponsibleParty_MarketParticipant_mRID": ["f"],
            "InMeteringGridArea_Domain_mRID": [inmeteringgridarea_domain_mrid],
            "InMeteringGridArea_Domain_Owner_mRID": [inmeteringgridownerarea_domain_mrid],
            "OutMeteringGridArea_Domain_mRID": [outmeteringgridarea_domain_mrid],
            "OutMeteringGridArea_Domain_Owner_mRID": [outmeteringgridownerarea_domain_mrid],
            "Parent_Domain_mRID": ["j"],
            "ServiceCategory_Kind": ["l"],
            "MarketEvaluationPointType": [market_evaluation_point_type],
            "SettlementMethod": [settlement_method],
            "QuantityMeasurementUnit_Name": ["o"],
            "Product": ["p"],
            "Technology": ["t"]})
        return spark.createDataFrame(pandas_df, schema=master_schema)
    return factory


@pytest.fixture(scope="session")
def parsed_data_factory(spark, parsed_schema):
    def factory(quantity):
        pandas_df = pd.DataFrame({
            "MarketEvaluationPoint_mRID": ["1"],
            "ObservationTime": [timestamp_now],
            "Quantity": [quantity],
            "CorrelationId": ["a"],
            "MessageReference": ["b"],
            "MarketDocument_mRID": ["c"],
            "CreatedDateTime": [timestamp_now],
            "SenderMarketParticipant_mRID": ["d"],
            "ProcessType": ["e"],
            "SenderMarketParticipantMarketRole_Type": ["f"],
            "TimeSeries_mRID": ["g"],
            "MktActivityRecord_Status": ["h"],
            "Product": ["i"],
            "QuantityMeasurementUnit_Name": ["j"],
            "MarketEvaluationPointType": [MarketEvaluationPointType.consumption.value],
            "Quality": [Quality.as_read.value],
            "EventHubEnqueueTime": [timestamp_now]})
        return spark.createDataFrame(pandas_df, schema=parsed_schema)
    return factory


@pytest.fixture(scope="session")
def enriched_data_factory(parsed_data_factory, master_data_factory):
    def creator(quantity=1.0,
                market_evaluation_point_type="m",
                settlement_method="n",
                meteringgridarea_domain_mrid="101",
                marketparticipant_mrid="11",
                inmeteringgridarea_domain_mrid="2",
                inmeteringgridownerarea_domain_mrid="3",
                outmeteringgridarea_domain_mrid="4",
                outmeteringgridownerarea_domain_mrid="5"):
        parsed_data = parsed_data_factory(quantity)
        master_data = master_data_factory(market_evaluation_point_type,
                                          marketparticipant_mrid,
                                          meteringgridarea_domain_mrid,
                                          inmeteringgridarea_domain_mrid,
                                          inmeteringgridownerarea_domain_mrid,
                                          outmeteringgridarea_domain_mrid,
                                          outmeteringgridownerarea_domain_mrid,
                                          settlement_method)
        return enrich_master_data(parsed_data, master_data)
    return creator
