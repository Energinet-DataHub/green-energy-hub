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
Delta Lake PoC Soft Delete Maual Tests

This file constains a number of tests to test soft delete in the Delta Lake PoC.
The tests demonstrates that it is possible to soft delete old time series values,
that it is possible to get the recent values without the soft deleted values and
that it is possible to go back in time and retrieve the soft deleted values again.
The tests also demonstrates, that it is possible to get the history of a single
metering point with or without the soft deleted values.

The main strategy for soft delete is to use pseudo time series values marked with a
"Soft Deleted" state. These pseudo values will shadow the real soft deleted values
when asked for the recent time series points. The pseudo values are also used when
asking for the history of a metering point either to filter out the soft deleted
values or to show them with a "Deleted" state.

Delta Lake layout:
  meteringPointId:
  timeSeriesId:
  timeSeriesPointId:
  observationDateTime:
  receivedDateTime: When the time series was received by DataHub
  meteringValue:
  quality: Fx Estimated, Measured, Soft Delete

Many of the cells with select requests are duplicated more than once.
The idea is, that the cells may be run from start to end starting on an empty storage.
Time series and soft deletes will then be added one at a time and the following cells
will show the effect when selecting recent time series values or metering point history
and verify that it is still possible to retrieve the old values.
"""

# %%
# Initalize Delta Lake PoC

from datetime import datetime, timedelta
from poc import DeltaLakePOC

poc = DeltaLakePOC()

start_time = datetime(2020, 1, 1, 1)
day1 = start_time
day2 = day1 + timedelta(days=1)
day3 = day2 + timedelta(days=1)
day4 = day3 + timedelta(days=1)
day5 = day4 + timedelta(days=1)

obs_time_1 = day1
obs_time_2 = obs_time_1 + timedelta(hours=1)
obs_time_3 = obs_time_2 + timedelta(hours=1)

# %%
# Insert some initial time series data for observation time 1, 2 and 3 at day1

rec_time_1 = obs_time_3
time_series_1 = [("MP1", "TS1", "P1", obs_time_1, rec_time_1, 111, "Measured"),
                 ("MP1", "TS1", "P2", obs_time_2, rec_time_1, 112, "Estimated"),
                 ("MP1", "TS1", "P3", obs_time_3, rec_time_1, 113, "Estimated")]


poc.add_time_series_to_delta_lake(time_series_1)

rec_time_2 = obs_time_3
time_series_2 = [("MP2", "TS2", "P4", obs_time_1, rec_time_2, 211, "Measured")]

poc.add_time_series_to_delta_lake(time_series_2)

print("OK")

# %%
# Retrieve recent time series point values, without soft deleted values

df = poc.select_recent_values()

df.show()

# %%
# Insert new time series with correction time series point for observation time 2 and 3 at day2

rec_time_3 = day2
time_series_3 = [("MP1", "TS3", "P5", obs_time_2, rec_time_3, 122, "Measured"),
                 ("MP1", "TS3", "P6", obs_time_3, rec_time_3, 123, "Measured")]

poc.add_time_series_to_delta_lake(time_series_3)

print("OK")

# %%
# Retrieve recent time series point values, without soft deleted values

df = poc.select_recent_values()

df.show()

# %%
# Retrieve the time series points before last update. That is the time series points
# added at day1

before_first_update = day1 + timedelta(hours=10)
df = poc.select_values_before_timestamp(before_first_update)

df.show()

# %%
# Retrieve the time series points for metering point MP1 added by time series TS3.

df = poc.select_values_for_metering_point_time_series("MP1", "TS3")

df.show()

# %%
# Retrieve all time series point values for metering point MP1

df = poc.select_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve history time series point values for metering point MP1 at observation time 2

df = poc.select_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve history time series points for metering point MP1 with a "Deleted" state for soft deleted values

df = poc.select_processed_history_for_metering_point("MP1")

df.show()

# %%
# Soft delete time the series points for observation time 2 with timestamp equal day3

soft_del_time_1 = day3

poc.soft_delete_period("MP1", obs_time_2, obs_time_2, soft_del_time_1)

print("OK")

# %%
# Retrieve recent time series point values, without soft deleted values

df = poc.select_recent_values()

df.show()

# %%
# Retrieve recent time series point values, with soft delete pseudo values

df = poc.select_recent_values_with_soft_delete_values()

df.show()

# %%
# Retrieve the time series points before first soft delete.

before_soft_del_1 = day2 + timedelta(hours=1)
df = poc.select_values_before_timestamp(before_soft_del_1)

df.show()

# %%
# Soft delete time series points for observation time 3 with timestamp equal day4

soft_del_time_2 = day4

poc.soft_delete_period("MP1", obs_time_3, obs_time_3, soft_del_time_2)

print("OK")

# %%
# Retrieve recent time series point values, without soft deleted values

df = poc.select_recent_values()

df.show()

# %%
# Retrieve recent time series point values, with soft delete pseudo values

df = poc.select_recent_values_with_soft_delete_values()

df.show()

# %%
# Retrieve recent time series point values for metering point MP1.

df = poc.select_recent_values_for_metering_point("MP1")

df.show()

# %%
# Retrieve the time series points before last soft delete.

before_soft_del_2 = day3 + timedelta(hours=10)
df = poc.select_values_before_timestamp(before_soft_del_2)

df.show()

# %%
# Retrieve the time series points for metering point MP1 before last soft delete.
# That is the time series points for metering point MP1 just after the first soft delete.

before_soft_del_2 = day3 + timedelta(hours=10)
df = poc.select_values_for_metering_point_before_timestamp("MP1", before_soft_del_2)

df.show()

# %%
# Retrieve the time series points for metering point MP1 before first update.
# That is the time series points for metering point MP1 inserted af day1.

before_first_update = day1 + timedelta(hours=10)
df = poc.select_values_for_metering_point_before_timestamp("MP1", before_first_update)

df.show()

# %%
# Retrieve all time series point values for metering point MP1 without soft deleted values

df = poc.select_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve history time series points for metering point MP1 and observation time 2
# The result is an empty list as the time series points for observation time 2 have
# been soft deleted

df = poc.select_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve history time series points for metering point MP1 with a "Deleted" state for soft deleted values

df = poc.select_processed_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve histpry time series points for metering point MP1 at observation time 2
# with a "Deleted" state for soft deleted values

df = poc.select_processed_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve all history time series points for metering point MP1
# including soft deleted values and including soft delete pseudo time series points

df = poc.select_all_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve all history time series points for metering point MP1 including soft deleted values
# and soft delete pseudo time series points

df = poc.select_all_history_for_metering_point("MP1")

df.show()


# %%
# Insert new time series points for metering point MP1 and observation time 2 and 3 at day5

rec_time_4 = day5
time_series_4 = [("MP1", "TS4", "P7", obs_time_2, rec_time_4, 132, "Measured"),
                 ("MP1", "TS4", "P8", obs_time_3, rec_time_4, 133, "Measured")]

poc.add_time_series_to_delta_lake(time_series_4)

print("OK")

# %%
# Retrieve recent time series point values, without soft deleted values

df = poc.select_recent_values()

df.show()

# %%
# Retrieve all time series point values for metering point MP1

df = poc.select_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve history time series points for metering point MP1 and observation time 2

df = poc.select_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve all time series point values for metering point MP1 with a "Deleted" state for soft deleted values

df = poc.select_processed_history_for_metering_point("MP1")

df.show()

# %%
# Retrieve history time series points for metering point MP1 at observation time 2
# with a "Deleted" state for soft deleted values

df = poc.select_processed_history_for_metering_point_observation_time("MP1", obs_time_2)

df.show()

# %%
# Retrieve all time series point values from Delta Lake. Use for debugging

df = poc.get_all_content()

df.show()

# %%
