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
import pandas as pd
import time
from datetime import datetime, timedelta
import pytest

from geh_stream.streaming_utils import parse_enrich_and_validate_time_series_as_points
from geh_stream.streaming_utils.streamhandlers.enrichers import enrich_master_data
from geh_stream.streaming_utils.streamhandlers import denormalize_parsed_data


def __create_time_stamp(offset_datetime, minutes_offset: int):
    new_time = offset_datetime + timedelta(minutes=minutes_offset)
    return pd.Timestamp(new_time, unit='s')


offset_time = datetime.now()

# Simulate two master data intervals (with different data)
valid_from1 = __create_time_stamp(offset_time, 0)
valid_to1 = __create_time_stamp(offset_time, 60)  # ValidTo of first interval and ValidFrom of second interval
valid_to2 = __create_time_stamp(offset_time, 120)


@pytest.fixture(scope="class")
def master_data(master_data_factory):
    """
    Create two master data intervals. Column 'technology' is arbitrary selected as a flag to make it indetifyable,
    which interval was used in enrichment.
    """
    return master_data_factory([
        dict(market_evaluation_point_mrid="1", valid_from=valid_from1, valid_to=valid_to1, technology="1"),
        dict(market_evaluation_point_mrid="1", valid_from=valid_to1, valid_to=valid_to2, technology="2")
    ])


@pytest.fixture(scope="class")
def enriched_data_factory(master_data, parsed_data_factory):
    def __factory(**kwargs):
        parsed_data = parsed_data_factory(kwargs)
        denormalized_parsed_data = denormalize_parsed_data(parsed_data)
        return enrich_master_data(denormalized_parsed_data, master_data)
    return __factory


def test_valid_from_is_inclusive(enriched_data_factory):
    enriched_data = enriched_data_factory(market_evaluation_point_mrid="1", observation_time=valid_from1)
    print(str(enriched_data.first()))
    assert enriched_data.first().Technology == "1"


def test_valid_to_is_exclusive(enriched_data_factory):
    # Act
    enriched_data = enriched_data_factory(market_evaluation_point_mrid="1", observation_time=valid_to1)

    # Assert: Only one of the intervals was used in join (previously we had an error where the time series point
    # multiplied because it was matched by both the end of first interval and beginning of second interval).
    assert enriched_data.count() == 1

    # Assert: The time series point was matched with the second interval (when it matches both the end of first
    # interval and the begining of the second interval).
    assert enriched_data.first().Technology == "2"
