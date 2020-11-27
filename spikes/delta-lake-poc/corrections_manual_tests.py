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
Delta Lake Proof of Concept
Manual tests for handling of correction time series points

This file constains a number of tests to test the Delta Lake PoC.
The tests demonstrates that it is possible to update time series points
in Delta Lake. That it is possible to retrieve the latest values without
the overwritten obsolete values, and that it is possible to get both the
old and new values when retrieving history data.
"""

# %%
# Initalize Delta Lake PoC

from datetime import datetime, timedelta
from time import sleep
from poc import DeltaLakePOC

poc = DeltaLakePOC()

start_time = datetime(2020, 1, 1, 1)
day1 = start_time
day2 = day1 + timedelta(days=1)

obs_time_1 = day1
obs_time_2 = obs_time_1 + timedelta(hours=1)

# %%
# Insert some initial data at day1

rec_time_1 = day1
time_series_1 = [("MP1", "TS1", "P1", obs_time_1, rec_time_1, 111, "Estimated")]

poc.add_time_series_to_delta_lake(time_series_1)

time_series_2 = [("MP2", "TS1", "P2", obs_time_1, rec_time_1, 112, "Measured")]
poc.add_time_series_to_delta_lake(time_series_2)

print("OK")

# %%
# Insert new time series points and correction time series point at day2

rec_time_2 = day2
time_series_3 = [("MP1", "TS2", "P3", obs_time_1, rec_time_2, 121, "Measured"),
                 ("MP1", "TS2", "P4", obs_time_2, rec_time_2, 122, "Measured")]

poc.add_time_series_to_delta_lake(time_series_3)

print("OK")

# %%
# Retrieve recent time series point values, without the overwritten values

df = poc.select_recent_values()

df.show()

# %%
# Retrieve the time series points before last update. That is the time series points
# added at time now1

before_last_update = day1 + timedelta(hours=10)
df = poc.select_values_before_timestamp(before_last_update)

df.show()

# %%
# Retrieve recent time series point values for Metering Point 1.
# That is without the overwritten values inserted at now1.

df = poc.select_recent_values_for_metering_point("MP1")

df.show()

# %%
# Retrieve the time series points for Metering Point 1 before last update.
# That is the time series points for Metering Point 1 inserted af now1.

before_last_update = day1 + timedelta(hours=10)
df = poc.select_values_for_metering_point_before_timestamp("MP1", before_last_update)

df.show()

# %%
# Retrieve all time series point values for Metering Point 1

df = poc.select_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve all time series point values for Metering Point 1 and Observation Time now1

df = poc.select_history_for_metering_point_observation_time("MP1", obs_time_1)

df.show()

# %%
# Retrieve all time series point values from Delta Lake. Use for debugging

df = poc.get_all_content()

df.show()

# %%
