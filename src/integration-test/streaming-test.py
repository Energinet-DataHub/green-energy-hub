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

"""How to run:
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
CORRELATION_ID_IN_FILE = "5c5d3f9e-eabf-46d9-8f86-f88d2de1f16d"

storage_account_name = sys.argv[1]
storage_account_key = sys.argv[2]
storage_container_name = sys.argv[3]
input_eh_connection_string = sys.argv[4]
delta_lake_output_path = sys.argv[5]

spark = spark_helper.get_spark_session(storage_account_name, storage_account_key)
delta_lake_base_path = spark_helper.get_base_storage_path(storage_container_name, storage_account_name)

import pathlib
script_dir = str(pathlib.Path(__file__).parent.absolute())
valid_time_series_message_json_path = script_dir + "/helper_files/valid_time_series_message.json"


async def test_load_valid_single_point_timeseries_value_into_eventhub():
    # Arrange
    column_name = "CorrelationId"
    valid_time_series_message = file_helper.read_file_as_string(valid_time_series_message_json_path)
    correlation_id = str(uuid.uuid4())  # Make sure correlation ID is unique to avoid interference with others
    valid_time_series_message_with_unique_correlation = valid_time_series_message.replace(CORRELATION_ID_IN_FILE, correlation_id)

    # Act
    await eventhub_helper.insert_content_on_eventhub(input_eh_connection_string, valid_time_series_message_with_unique_correlation)

    # Assert
    stored_value = spark_helper.get_stored_value_in_deltalake(spark, TIMEOUT_IN_MINUTES, delta_lake_base_path + delta_lake_output_path, column_name, correlation_id)
    assert stored_value is not None

# Eventhub requires async https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-python-get-started-send#send-events
loop = asyncio.get_event_loop()
loop.run_until_complete(test_load_valid_single_point_timeseries_value_into_eventhub())
loop.close()
