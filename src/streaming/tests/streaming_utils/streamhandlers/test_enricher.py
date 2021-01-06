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
import pandas as pd
import time
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import DecimalType, StringType, StructField, StructType, TimestampType, ArrayType
from pyspark.sql.functions import col

from geh_stream.schemas import SchemaNames, SchemaFactory
from geh_stream.streaming_utils.streamhandlers import Enricher
from geh_stream.dataframelib import has_column
from geh_stream.streaming_utils.streamhandlers import denormalize_parsed_data

# Create timestamps used in DataFrames
time_now = time.time()
time_future = time.time() + 1000
time_past = time.time() - 1000
time_far_future = time.time() + 10000
timestamp_now = pd.Timestamp(time_now, unit='s')
timestamp_future = pd.Timestamp(time_future, unit='s')
timestamp_past = pd.Timestamp(time_past, unit='s')
timestamp_far_future = pd.Timestamp(time_far_future, unit='s')


# Run the enrich function
@pytest.fixture(scope="class")
def enriched_data(parsed_data_factory, master_data_factory):
    parsed_data = parsed_data_factory([
        dict(market_evaluation_point_mrid="1", quantity=1.0, observation_time=timestamp_now),
        # Not matched because it's outside the master data valid period
        dict(market_evaluation_point_mrid="1", quantity=2.0, observation_time=timestamp_far_future),
        # Not matched because no master data exists for this market evalution point
        dict(market_evaluation_point_mrid="2", quantity=3.0, observation_time=timestamp_now)
    ])
    denormalized_parsed_data = denormalize_parsed_data(parsed_data)
    master_data = master_data_factory(market_evaluation_point_mrid="1")
    return Enricher.enrich(denormalized_parsed_data, master_data)


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


def test_enricher_adds_meter_reading_periodicity(enriched_data):
    assert has_column(enriched_data, "md.MeterReadingPeriodicity")


def test_enricher_adds_metering_method(enriched_data):
    assert has_column(enriched_data, "md.MeteringMethod")


def test_enricher_adds_metering_grid_area_domain_mrid(enriched_data):
    assert has_column(enriched_data, "md.MeteringGridArea_Domain_mRID")


def test_enricher_adds_connection_state(enriched_data):
    assert has_column(enriched_data, "md.ConnectionState")


def test_enricher_adds_energysupplier_marketParticipant_mrid(enriched_data):
    assert has_column(enriched_data, "md.EnergySupplier_MarketParticipant_mRID")


def test_enricher_adds_balance_responsible_party_market_participant_mrid(enriched_data):
    assert has_column(enriched_data, "md.BalanceResponsibleParty_MarketParticipant_mRID")


def test_enricher_adds_in_metering_grid_area_domain_mrid(enriched_data):
    assert has_column(enriched_data, "md.InMeteringGridArea_Domain_mRID")


def test_enricher_adds_in_metering_grid_area_domain_owner_mrid(enriched_data):
    assert has_column(enriched_data, "md.InMeteringGridArea_Domain_Owner_mRID")


def test_enricher_adds_out_metering_grid_area_domain_mrid(enriched_data):
    assert has_column(enriched_data, "md.OutMeteringGridArea_Domain_mRID")


def test_enricher_adds_out_metering_grid_area_domain_owner_mrid(enriched_data):
    assert has_column(enriched_data, "md.OutMeteringGridArea_Domain_Owner_mRID")


def test_enricher_adds_parent_domain_mrid(enriched_data):
    assert has_column(enriched_data, "md.Parent_Domain_mRID")


def test_enricher_adds_service_category_kind(enriched_data):
    assert has_column(enriched_data, "md.ServiceCategory_Kind")


def test_enricher_adds_market_evaluation_point_type(enriched_data):
    assert has_column(enriched_data, "md.MarketEvaluationPointType")


def test_enricher_adds_settlement_method(enriched_data):
    assert has_column(enriched_data, "md.SettlementMethod")


def test_enricher_adds_quantity_measurement_unit_Name(enriched_data):
    assert has_column(enriched_data, "md.QuantityMeasurementUnit_Name")


def test_enricher_adds_product(enriched_data):
    print("enriched_data in product test")
    enriched_data.show()
    assert has_column(enriched_data, "md.Product")


def test_enricher_adds_technology(enriched_data):
    assert has_column(enriched_data, "md.Technology")
