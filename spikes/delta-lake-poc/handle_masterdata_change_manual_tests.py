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
Delta Lake POC for master data change back in time - manual tests

This file constains a number of cells to demonstrate the master data
supplier change POC.
The input data variables (fromDate, metering_point, old_supplier,
new_supplier) are expected to come from the master data part of the
architecture once the change supplier business process is implemented.
"""

#%% Initialize
from datetime import datetime
from masterdata import HandleMasterdataChange

poc = HandleMasterdataChange()

#Input data (will be provided by master data part of architecture when supplier has been changed back in time)
start_date_time = datetime(2020, 11, 18, 0, 0, 0)
metering_point = "MP1"
old_supplier = "supplierA"
new_supplier = "supplierB"

#%% Fetch timeseries points from delta lake based on start time and metering point ID
timeseries_df = poc.get_time_series_point_for_metering_point_since_timestamp(metering_point, start_date_time)
timeseries_df.show()

#%% Fetch currently valid timeseries points since timestamp for metering point
timeseries_df = poc.get_current_valid_time_series_point_for_metering_point_since_timestamp(metering_point, start_date_time)
timeseries_df.show()

#%% Update supplier on retrieved dataset
updatedDataset = poc.change_supplier_of_time_series_point(timeseries_df, old_supplier, new_supplier)
updatedDataset.show()

# %% Save updated dataset
poc.add_time_series_to_delta_lake(updatedDataset)
# At this point the time series points back in time have have been created in Delta lake. The points are ready for being re-send

#%% Create simple test dataset
valid_from_1 = datetime(2020, 11, 17, 3, 0, 0)
valid_from_2 = datetime(2020, 11, 18, 3, 0, 0)
observation_time_1 = datetime(2020, 11, 17, 1, 0, 0)
observation_time_2 = datetime(2020, 11, 17, 2, 0, 0)
observation_time_3 = datetime(2020, 11, 18, 1, 0, 0)
observation_time_4 = datetime(2020, 11, 18, 2, 0, 0)

mp1_ts = []
mp1_ts.append(poc.create_test_element(observation_time_1, "supplierA", valid_from_1))
mp1_ts.append(poc.create_test_element(observation_time_2, "supplierA", valid_from_1))
mp1_ts.append(poc.create_test_element(observation_time_3, "supplierA", valid_from_2))
mp1_ts.append(poc.create_test_element(observation_time_4, "supplierA", valid_from_2))

poc.add_test_data_time_series_to_delta_lake(mp1_ts)
# %%
