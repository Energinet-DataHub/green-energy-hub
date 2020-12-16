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
from pyspark.sql.functions import col, lit, to_timestamp, explode
from geh_stream.codelists import MarketEvaluationPointType, Quality
from geh_stream.streaming_utils import Enricher
from geh_stream.schemas import SchemaNames, SchemaFactory
from geh_stream.dataframelib import flatten_df
from geh_stream.streaming_utils.denormalization import denormalize_parsed_data


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
    def factory(market_evaluation_point_mrid="mepm",
                market_evaluation_point_type="mept",
                marketparticipant_mrid="mm",
                meteringgridarea_domain_mrid="mdm",
                inmeteringgridarea_domain_mrid="idm",
                inmeteringgridownerarea_domain_mrid="idm",
                outmeteringgridarea_domain_mrid="odm",
                outmeteringgridownerarea_domain_mrid="odm",
                settlement_method="sm"):
        pandas_df = pd.DataFrame({
            'MarketEvaluationPoint_mRID': [market_evaluation_point_mrid],
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
def time_series_json_factory():
    def factory(market_evaluation_point_mrid="mepm",
                quantity=1.0,
                observation_time=timestamp_now):
        json_str = """
    {{
        "TimeSeries_mRID": "g",
        "MessageReference": "b",
        "MarketDocument": {{
            "mRID": "c",
            "Type": "x",
            "CreatedDateTime": "{0}",
            "SenderMarketParticipant": {{
                "mRID": "x",
                "Type": "x"
            }},
            "RecipientMarketParticipant": {{
                "mRID": {{
                    "value": "x"
                }},
                "Type": "x"
            }},
            "ProcessType": "e",
            "MarketServiceCategory_Kind": "x"
        }},
        "MktActivityRecord_Status": "h",
        "Product": "i",
        "QuantityMeasurementUnit_Name": "j",
        "MarketEvaluationPointType": "{1}",
        "SettlementMethod": "x",
        "MarketEvaluationPoint_mRID": "{4}",
        "CorrelationId": "a",
        "Period": {{
            "Resolution": "x",
            "TimeInterval_Start": "{0}",
            "TimeInterval_End": "{0}",
            "Points": [
                {{
                    "Quantity": "{2}",
                    "Quality": "{3}",
                    "ObservationTime": "{5}"
                }}
            ]
        }}
    }}
    """.format(timestamp_now.isoformat() + "Z",
               MarketEvaluationPointType.consumption.value,
               quantity,
               Quality.as_read.value,
               market_evaluation_point_mrid,
               observation_time)
        print("json_str:")
        print(json_str)
        return json_str

    return factory


@pytest.fixture(scope="session")
def time_series_json(time_series_json_factory):
    return time_series_json_factory("mepm", 1.0)


@pytest.fixture(scope="session")
def parsed_data_factory(spark, parsed_schema, time_series_json_factory):
    def factory(arg):
        """
        Accepts either a dictionary in which case a single row is created,
        or accepts a list of dictionaries in which case a set of rows are created.
        """
        if not isinstance(arg, list):
            arg = [arg]

        json_strs = []
        for dic in arg:
            json_strs.append(time_series_json_factory(**dic))
        json_array_str = "[{0}]".format(", ".join(json_strs))
        json_rdd = spark.sparkContext.parallelize([json_array_str])
        parsed_data = spark.read.json(json_rdd,
                                      schema=None,
                                      dateFormat="yyyy-MM-dd'T'HH:mm:ss.SSSSSSS'Z'")
        print("parsed_data:")
        parsed_data.printSchema()
        parsed_data.show()
        return parsed_data

    return factory


@pytest.fixture(scope="session")
def parsed_data(parsed_data_factory):
    return parsed_data_factory(dict())


@pytest.fixture(scope="session")
def enriched_data_factory(parsed_data_factory, master_data_factory):
    def creator(market_evaluation_point_mrid="mepm",
                quantity=1.0,
                market_evaluation_point_type="m",
                settlement_method="n",
                meteringgridarea_domain_mrid="101",
                marketparticipant_mrid="11",
                inmeteringgridarea_domain_mrid="2",
                inmeteringgridownerarea_domain_mrid="3",
                outmeteringgridarea_domain_mrid="4",
                outmeteringgridownerarea_domain_mrid="5"):
        parsed_data = parsed_data_factory(dict(market_evaluation_point_mrid=market_evaluation_point_mrid, quantity=quantity))
        denormalized_parsed_data = denormalize_parsed_data(parsed_data)
        master_data = master_data_factory(market_evaluation_point_mrid,
                                          market_evaluation_point_type,
                                          marketparticipant_mrid,
                                          meteringgridarea_domain_mrid,
                                          inmeteringgridarea_domain_mrid,
                                          inmeteringgridownerarea_domain_mrid,
                                          outmeteringgridarea_domain_mrid,
                                          outmeteringgridownerarea_domain_mrid,
                                          settlement_method)
        return Enricher.enrich(denormalized_parsed_data, master_data)
    return creator


@pytest.fixture(scope="session")
def enriched_data(enriched_data_factory):
    return enriched_data_factory()
