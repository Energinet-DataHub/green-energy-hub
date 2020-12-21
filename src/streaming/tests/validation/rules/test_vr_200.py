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
from geh_stream.validation.rules.vr_200 import validate_vr_200


def test_vr_200_is_valid(enriched_data):
    validated_data = validate_vr_200(enriched_data)
    assert validated_data.first()["VR-200-Is-Valid"]


def test_vr_200_is_invalid_when_market_evaluation_point_is_missing(non_enriched_data):
    validated_data = validate_vr_200(non_enriched_data)
    assert not validated_data.first()["VR-200-Is-Valid"]
