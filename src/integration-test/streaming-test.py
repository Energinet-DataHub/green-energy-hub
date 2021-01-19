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
import sys
import asyncio
import uuid
from helpers import spark_helper, eventhub_helper, file_helper

"""
HOW TO RUN THE TEST(S):
To run it execute the test 5 arguments need to be submitted when executing

storage_account_name = sys.argv[1]
storage_account_key = sys.argv[2]
storage_container_name = sys.argv[3]
input_eh_connection_string = sys.argv[4]
delta_lake_output_path = sys.argv[5]

Example: python streaming-test.py storage_account_name storage_account_key storage_container_name "input_eh_connection_string" "delta/meter-data"

Remember to put "" around input_eh_connection_string

In addition, the streaming and validation job must be running. If running manually you can execute the following
command from /src/streaming in a new terminal:

    python enrichment_and_validation.py
"""

TIMEOUT_IN_MINUTES = 5
CORRELATION_ID_IN_FILE = "WILL BE REPLACED BY INTEGRATION TEST"

storage_account_name = sys.argv[1]
storage_account_key = sys.argv[2]
storage_container_name = sys.argv[3]
input_eh_connection_string = sys.argv[4]
delta_lake_output_path = sys.argv[5]

spark = spark_helper.get_spark_session(storage_account_name, storage_account_key)
delta_lake_base_path = spark_helper.get_base_storage_path(storage_container_name, storage_account_name)

import datetime
from decimal import Decimal
import pathlib
script_dir = str(pathlib.Path(__file__).parent.absolute())
consumption_time_series_message_json_path = script_dir + "/helper_files/valid_time_series_message.json"
exchange_time_series_message_json_path = script_dir + "/helper_files/valid_time_series_message_for_exchange.json"


def __get_time_series_from_delta_lake_by_correlation_id(correlation_id):
    return spark_helper.get_stored_value_in_deltalake(
        spark,
        TIMEOUT_IN_MINUTES,
        delta_lake_base_path + delta_lake_output_path,
        "CorrelationId",
        correlation_id)


async def test_streaming_adds_valid_time_series_with_all_required_data_to_delta_lake():
    """
    This is an integration test, which tests that the streaming job stores all required streamed and enriched fields
    of valid time series in Delta Lake for aggregations.

    It is worth to note that test sends to different messages (one for at consumption market evalution point and one
    for an exchange point) in order to observe values in all fields.

    Please don't update this test without ensuring that necessary changes to Delta Lake data format has
    also been made.
    """

    # Arrange
    consumption_time_series_template = file_helper.read_file_as_string(consumption_time_series_message_json_path)
    consumption_correlation_id = str(uuid.uuid4())  # Make sure correlation ID is unique to avoid interference with others
    consumption_time_series_message = consumption_time_series_template.replace(CORRELATION_ID_IN_FILE, consumption_correlation_id)

    exchange_time_series_template = file_helper.read_file_as_string(exchange_time_series_message_json_path)
    exchange_correlation_id = str(uuid.uuid4())  # Make sure correlation ID is unique to avoid interference with others
    exchange_time_series_message = exchange_time_series_template.replace(CORRELATION_ID_IN_FILE, exchange_correlation_id)

    # Act
    await eventhub_helper.insert_content_on_eventhub(input_eh_connection_string, consumption_time_series_message)
    await eventhub_helper.insert_content_on_eventhub(input_eh_connection_string, exchange_time_series_message)

    # Assert
    consumption_time_series_points = __get_time_series_from_delta_lake_by_correlation_id(consumption_correlation_id)
    point = next(p for p in consumption_time_series_points if lambda p: p.Time == datetime.datetime(2020, 11, 13, 0, 0))

    assert point.MarketEvaluationPoint_mRID == "571313180000000005"
    assert point.Time == datetime.datetime(2020, 11, 13, 0, 0)
    assert point.Quantity == Decimal("2")
    assert point.CorrelationId == consumption_correlation_id
    assert point.MessageReference == "mId2020-12-01T13:16:29.330Z"
    assert point.MarketDocument_mRID == "hId2020-12-01T13:16:29.330Z"
    assert point.CreatedDateTime == datetime.datetime(2020, 12, 1, 13, 16, 29, 330000)
    assert point.SenderMarketParticipant_mRID == "8100000000030"
    assert point.ProcessType == "D42"
    assert point.SenderMarketParticipantMarketRole_Type == "MDR"
    assert point.TimeSeries_mRID == "tId2020-12-01T13:16:29.330Z"
    assert point.MktActivityRecord_Status == "9"
    assert point.MarketEvaluationPointType == "E17"
    assert point.Quality == "E01"
    assert point.MeterReadingPeriodicity == "PT1H"
    assert point.MeteringMethod == "D01"
    assert point.MeteringGridArea_Domain_mRID == "800"
    assert point.ConnectionState == "E22"
    assert point.EnergySupplier_MarketParticipant_mRID == "8100000000108"
    assert point.BalanceResponsibleParty_MarketParticipant_mRID == "8100000000207"
    assert point.InMeteringGridArea_Domain_mRID is None
    assert point.OutMeteringGridArea_Domain_mRID is None
    assert point.Parent_Domain_mRID is None
    assert point.ServiceCategory_Kind == "23"
    assert point.SettlementMethod == "D01"
    assert point.QuantityMeasurementUnit_Name == "KWH"
    assert point.Product == "8716867000030"
    assert point.year == 2020
    assert point.month == 11
    assert point.day == 13

    exchange_time_series_points = __get_time_series_from_delta_lake_by_correlation_id(exchange_correlation_id)
    exchange_point = next(p for p in exchange_time_series_points if lambda p: p.Time == datetime.datetime(2020, 12, 6, 0, 0))

    # These are the only fields we couldn't test with the consumption message - no need to assert the rest of the fields again.
    assert exchange_point.InMeteringGridArea_Domain_mRID == "802"
    assert exchange_point.OutMeteringGridArea_Domain_mRID == "803"
    assert exchange_point.Parent_Domain_mRID == "571313180000000074"


# Eventhub requires async https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-python-get-started-send#send-events
loop = asyncio.get_event_loop()
loop.run_until_complete(test_streaming_adds_valid_time_series_with_all_required_data_to_delta_lake())
loop.close()
