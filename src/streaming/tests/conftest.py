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
from pyspark.sql.functions import col, lit, to_timestamp, explode
from pyspark.sql.types import StructType, StructField, StringType, ArrayType, DecimalType, TimestampType, BooleanType
import pandas as pd
from decimal import Decimal
from datetime import datetime
import time
import uuid


from geh_stream.codelists import MarketEvaluationPointType, Quality
from geh_stream.streaming_utils.streamhandlers import Enricher
from geh_stream.schemas import SchemaNames, SchemaFactory
from geh_stream.dataframelib import flatten_df
from geh_stream.streaming_utils.streamhandlers.denormalization import denormalize_parsed_data


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
            "OutMeteringGridArea_Domain_mRID": [outmeteringgridarea_domain_mrid],
            "Parent_Domain_mRID": ["j"],
            "ServiceCategory_Kind": ["l"],
            "MarketEvaluationPointType": [market_evaluation_point_type],
            "SettlementMethod": [settlement_method],
            "QuantityMeasurementUnit_Name": ["o"],
            "Product": ["p"],
            "Technology": ["t"],
            "OutMeteringGridArea_Domain_Owner_mRID": [outmeteringgridownerarea_domain_mrid],
            "InMeteringGridArea_Domain_Owner_mRID": [inmeteringgridownerarea_domain_mrid]})
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
                    "Time": "{5}"
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
                outmeteringgridownerarea_domain_mrid="5",
                do_fail_enrichment=False):
        parsed_data = parsed_data_factory(dict(market_evaluation_point_mrid=market_evaluation_point_mrid, quantity=quantity))
        denormalized_parsed_data = denormalize_parsed_data(parsed_data)

        # Should join find a matching master data record or not?
        # If so use a non matching mRID for the master data record.
        if do_fail_enrichment:
            non_matching_market_evaluation_point_mrid = str(uuid.uuid4())
            market_evaluation_point_mrid = non_matching_market_evaluation_point_mrid

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


@pytest.fixture(scope="session")
def non_enriched_data(enriched_data_factory):
    """Simulate data with no master data for market evaluation point."""
    return enriched_data_factory(do_fail_enrichment=True)


date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"
default_obs_time = datetime.strptime("2020-01-01T00:00:00+0000", date_time_formatting_string)


@pytest.fixture(scope="module")
def cosmos_write_config():
    writeConfig = {
        "Endpoint": "",
        "Masterkey": "",
        "Database": "",
        "Collection": "",
        "Upsert": "true"
    }
    return writeConfig


@pytest.fixture(scope="module")
def valid_atomic_value_schema():
    """
    Valid atomic data point schema to send
    """
    return StructType([
        StructField("IsTimeSeriesValid", BooleanType(), False),
        StructField("CorrelationId", StringType(), False),
        StructField("MarketEvaluationPoint_mRID", StringType(), False),
        StructField("MeterReadingPeriodicity", StringType(), False),
        StructField("Product", StringType(), False),
        StructField("QuantityMeasurementUnit_Name", StringType(), False),
        StructField("MarketEvaluationPointType", StringType(), False),
        StructField("SettlementMethod", StringType(), False),
        StructField("MarketDocument_ProcessType", StringType(), False),
        StructField("MarketDocument_RecipientMarketParticipant_Type", StringType(), False),
        StructField("MarketDocument_MarketServiceCategory_Kind", StringType(), False),
        StructField("RecipientList", ArrayType(StringType()), False),
        StructField("Period_Point_Quantity", SchemaFactory.quantity_type, False),
        StructField("Period_Point_Quality", StringType(), False),
        StructField("Period_Point_Time", TimestampType(), False),
        StructField("MarketDocument_CreatedDateTime", TimestampType(), False),
        StructField("EventHubEnqueueTime", TimestampType(), False)
    ])


@pytest.fixture(scope="module")
def valid_atomic_values_for_actors_sample_df(spark, valid_atomic_value_schema):
    structureData = [
        (True, "10024", "3456", "15min", "product1", "m", "pointtype", "smet", "pr_type", "actor_role", "SC_KIND", ["r1", "r2", "r3"], Decimal(20048), "", default_obs_time, default_obs_time, default_obs_time)
    ]
    df2 = spark.createDataFrame(data=structureData, schema=valid_atomic_value_schema)
    return df2


@pytest.fixture(scope="module")
def invalid_atomic_value_schema():
    """
    Valid atomic data point schema to send
    """
    return StructType([
        StructField("TimeSeries_mRID", StringType(), False),
        StructField("IsTimeSeriesValid", BooleanType(), False),
        StructField("CorrelationId", StringType(), False),
        StructField("MarketDocument_SenderMarketParticipant_mRID", StringType(), False),
        StructField("MarketDocument_SenderMarketParticipant_Type", StringType(), False),
        StructField("MarketDocument_mRID", StringType(), False),
        StructField("MarketDocument_ProcessType", StringType(), False),
        StructField("VR-245-1-Is-Valid", BooleanType(), False),
        StructField("VR-250-Is-Valid", BooleanType(), False),
        StructField("VR-251-Is-Valid", BooleanType(), False),
        StructField("VR-611-Is-Valid", BooleanType(), False),
        StructField("VR-612-Is-Valid", BooleanType(), False)
    ])


@pytest.fixture(scope="module")
def invalid_atomic_values_for_actors_sample_df(spark, invalid_atomic_value_schema):
    structureData = [
        ("tseries_id", False, "10024", "420901", "actor_role", "12345", "ptype", True, False, True, True, True)
    ]
    df2 = spark.createDataFrame(data=structureData, schema=invalid_atomic_value_schema)
    return df2


@pytest.fixture(scope="module")
def validation_results_schema():
    """
    Validation subset of columns
    """
    return StructType([
        StructField("VR-245-1-Is-Valid", BooleanType(), False),
        StructField("VR-250-Is-Valid", BooleanType(), False),
        StructField("VR-251-Is-Valid", BooleanType(), False),
        StructField("VR-611-Is-Valid", BooleanType(), False),
        StructField("VR-612-Is-Valid", BooleanType(), False)
    ])


@pytest.fixture(scope="module")
def validation_results_values_for_actors_sample_df(spark, validation_results_schema):
    structureData = [
        (True, True, True, True, True),
        (False, True, True, True, True),
        (True, False, True, True, True),
        (True, True, False, True, True),
        (True, True, True, False, True),
        (True, True, True, True, False),
        (False, False, False, False, False)
    ]
    df2 = spark.createDataFrame(data=structureData, schema=validation_results_schema)
    return df2
