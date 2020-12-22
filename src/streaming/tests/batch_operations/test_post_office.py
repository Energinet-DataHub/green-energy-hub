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
from geh_stream.batch_operations import PostOffice


def test_extractValidMessageAtomicValues(valid_atomic_values_for_actors_sample_df):
    postOffice = PostOffice()
    test_df = postOffice.extractValidMessageAtomicValues(valid_atomic_values_for_actors_sample_df)
    assert len(test_df.columns) == 17


def test_extractInvalidMessageAtomicValues(invalid_atomic_values_for_actors_sample_df):
    postOffice = PostOffice()
    test_df = postOffice.extractInvalidMessageAtomicValues(invalid_atomic_values_for_actors_sample_df)
    assert ("Reasons" in test_df.columns)
    assert len(test_df.columns) == 8


def test_extractReasons(validation_results_values_for_actors_sample_df):
    postOffice = PostOffice()
    test_df = postOffice.extractReasons(validation_results_values_for_actors_sample_df)

    assert ("Reasons" in test_df.columns)
    assert len(test_df.columns) == 6
