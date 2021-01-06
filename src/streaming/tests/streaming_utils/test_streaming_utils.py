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

from geh_stream.streaming_utils import parse_enrich_and_validate_time_series_as_points


@pytest.fixture(scope="session")
def valid_time_series_point(parsed_data_from_json_file_factory, master_data):
    time_series = parsed_data_from_json_file_factory("single_valid_time_series.template.json")
    time_series_points = parse_enrich_and_validate_time_series_as_points(time_series, master_data)
    return time_series_points.first()


@pytest.fixture(scope="session")
def vr200_invalid_time_series_point(parsed_data_from_json_file_factory, master_data):
    time_series = parsed_data_from_json_file_factory("vr200_invalid_time_series.template.json")
    time_series_points = parse_enrich_and_validate_time_series_as_points(time_series, master_data)
    return time_series_points.first()


def test_valid_time_series_first_point_has_enriched_data(valid_time_series_point):
    assert valid_time_series_point.EnergySupplier_MarketParticipant_mRID == "8100000000108"


def test_valid_time_series_first_point_has_streaming_data(valid_time_series_point):
    assert valid_time_series_point.mRID == "tId2020-12-01T13:16:29.330Z"


def test_valid_time_series_first_point_is_valid(valid_time_series_point):
    assert valid_time_series_point.IsTimeSeriesPointValid


def test_vr200_invalid_time_series_first_point_is_invalid(vr200_invalid_time_series_point):
    assert not vr200_invalid_time_series_point.IsTimeSeriesPointValid
    assert not vr200_invalid_time_series_point["VR-200-Is-Valid"]


def test_valid_time_series_first_point_has_observation_time(valid_time_series_point):
    assert valid_time_series_point.Period_Point_Time.isoformat() + "Z" == "2020-11-12T23:00:00Z"
