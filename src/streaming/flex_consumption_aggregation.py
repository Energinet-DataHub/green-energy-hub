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
Aggregate tempory aggregate flex settled consumption per balance supplier or balance responsible party
"""

# %%
import json
import time
import urllib.parse

import configargparse
from datetime import datetime
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession

# %%
p = configargparse.ArgParser(prog='flex_consumption_aggregation.py', description='Green Energy Hub Tempory aggregate flex settled consumption per balance supplier or balance responsible party Aggregation',
                             default_config_files=['configuration/run_args_flex_consumption.conf'],
                             formatter_class=configargparse.ArgumentDefaultsHelpFormatter)
p.add('--input-storage-account-name', type=str, required=True,
      help='Azure Storage account name holding time series data')
p.add('--input-storage-account-key', type=str, required=True,
      help='Azure Storage key for input storage', env_var='GEH_INPUT_STORAGE_KEY')
p.add('--input-storage-container-name', type=str, required=False, default='data',
      help='Azure Storage container name for input storage')
p.add('--input-path', type=str, required=False, default="delta/meter-data/",
      help='Path to time series data storage location (deltalake) relative to root container')
p.add('--output-storage-account-name', type=str, required=True,
      help='Azure Storage account name holding aggregations')
p.add('--output-storage-account-key', type=str, required=True,
      help='Azure Storage key for output storage', env_var='GEH_OUTPUT_STORAGE_KEY')
p.add('--output-storage-container-name', type=str, required=False, default='aggregations',
      help='Azure Storage container name for output storage')
p.add('--output-path', type=str, required=False, default="delta/flex_consumption/",
      help='Path to aggregation storage location (deltalake) relative to root container')
p.add('--beginning-date-time', type=str, required=True,
      help='The timezone aware date-time representing the beginning of the time period of aggregation (ex: 2020-01-03T00:00:00+0100)')
p.add('--end-date-time', type=str, required=True,
      help='The timezone aware date-time representing the end of the time period of aggregation (ex: 2020-01-03T00:00:00-0100)')
p.add('--telemetry-instrumentation-key', type=str, required=True,
      help='Instrumentation key used for telemetry')

args, unknown_args = p.parse_known_args()

# Parse the given date times
date_time_formatting_string = "%Y-%m-%dT%H:%M:%S%z"
end_date_time = datetime.strptime(args.end_date_time, date_time_formatting_string)
beginning_date_time = datetime.strptime(args.beginning_date_time, date_time_formatting_string)

if unknown_args:
    print("Unknown args:")
    _ = [print(arg) for arg in unknown_args]

# %%

# Set spark config with storage account names/keys and the session timezone so that datetimes are displayed consistently (in UTC)
spark_conf = SparkConf(loadDefaults=True) \
    .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(args.input_storage_account_name),
         args.input_storage_account_key) \
    .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(args.output_storage_account_name),
         args.output_storage_account_key) \
    .set("spark.sql.session.timeZone", "UTC")

spark = SparkSession\
    .builder\
    .config(conf=spark_conf)\
    .getOrCreate()

sc = spark.sparkContext
print("Spark Configuration:")
_ = [print(k + '=' + v) for k, v in sc.getConf().getAll()]

# %%
# Create input and output storage paths

INPUT_STORAGE_PATH = "wasbs://{0}@{1}.blob.core.windows.net/{2}".format(
    args.input_storage_container_name, args.input_storage_account_name, args.input_path
)

print("Input storage url:", INPUT_STORAGE_PATH)

OUTPUT_STORAGE_PATH = "wasbs://{0}@{1}.blob.core.windows.net/{2}".format(
    args.output_storage_container_name, args.output_storage_account_name, args.output_path
)

print("Output storage url:", OUTPUT_STORAGE_PATH)

# %%

# Read in time series data (delta doesn't support user specified schema)
timeseries_df = spark \
    .read \
    .format("delta") \
    .load(INPUT_STORAGE_PATH)

# %%
from geh_stream.aggregation_utils.filters import TimePeriodFilter

# Filter out time series data that is not in the specified time period
valid_time_period_df = TimePeriodFilter.filter(timeseries_df, beginning_date_time, end_date_time)

# %%
from geh_stream.aggregation_utils.aggregators import FlexConsumptionAggregator

# Perform aggregation calculation
aggregated_df = FlexConsumptionAggregator.aggregate(valid_time_period_df)

# %%

# Write out to delta storage
# TODO: discuss partitioning plan, if necessary - Issue #256
aggregated_df \
    .write \
    .format("delta") \
    .save(OUTPUT_STORAGE_PATH)

# %%
