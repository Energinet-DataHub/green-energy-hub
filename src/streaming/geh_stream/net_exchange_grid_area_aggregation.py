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
Aggregate net exchange per grid area
"""

# %%
import configargparse
from datetime import datetime
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import StructType, TimestampType, StringType, DoubleType
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit

# %%
p = configargparse.ArgParser(prog='net_exchange_grid_area_aggregation.py', description='Net exchange aggregation per grid area',
                             default_config_files=[
                                 'configuration/run_args_net_exchange_grid_area.conf'],
                             formatter_class=configargparse.ArgumentDefaultsHelpFormatter
                             )
p.add('--input-storage-account-name', type=str, required=False, default='stordatahub3datalaketest',
      help='Azure Storage account name (used for data output and checkpointing)')
p.add('--input-storage-account-key', type=str, required=False, default='***',
      help='Azure Storage key', env_var='GEH_STREAMING_STORAGE_KEY')
p.add('--input-storage-container-name', type=str, required=False, default='timeseriesdata',
      help='Azure Storage container name')
p.add('--input-path', type=str, required=False, default="delta/meter-data/",
      help='Path to time series data storage location (deltalake) relative to root container')

p.add('--output-storage-account-name', type=str, required=False, default='stordatahub3datalaketest',
      help='Azure Storage account name holding aggregations')
p.add('--output-storage-account-key', type=str, required=False,
      help='Azure Storage key for output storage', env_var='GEH_OUTPUT_STORAGE_KEY')
p.add('--output-storage-container-name', type=str, required=False, default='aggregations',
      help='Azure Storage container name for output storage')
p.add('--output-path', type=str, required=False, default="delta/net-exchange/",
      help='Path to aggregation storage location (deltalake) relative to root container')
p.add('--beginning-date-time', type=str, required=False, default='2020-01-03T00:00:00+0100',
      help='The timezone aware date-time representing the beginning of the time period of aggregation (ex: 2020-01-03T00:00:00+0100)')
p.add('--end-date-time', type=str, required=False, default='2020-01-03T00:00:00-0100',
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

spark_conf = SparkConf(loadDefaults=True) \
    .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(args.input_storage_account_name),
         args.input_storage_account_key) \
    .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(args.output_storage_account_name),
         args.output_storage_account_key)

spark = SparkSession\
    .builder\
    .config(conf=spark_conf)\
    .getOrCreate()

sc = spark.sparkContext
print("Spark Configuration:")
_ = [print(k + '=' + v) for k, v in sc.getConf().getAll()]

# %%

INPUT_STORAGE_PATH = "wasbs://{0}@{1}.blob.core.windows.net/messages".format(
    args.input_storage_container_name, args.input_storage_account_name
)

print("Input storage url:", INPUT_STORAGE_PATH)

OUTPUT_STORAGE_PATH = "wasbs://{0}@{1}.blob.core.windows.net/".format(
    args.output_storage_container_name, args.output_storage_account_name
)

print("Output storage url:", OUTPUT_STORAGE_PATH)

# %%

# read in time series data with schema
timeseries_df = spark \
    .read \
    .format("delta") \
    .load(INPUT_STORAGE_PATH)

# %%
from geh_stream.aggregation_utils.filters import TimePeriodFilter

# Filter out time series data that is not in the specified time period
valid_time_period_df = TimePeriodFilter.filter(timeseries_df, beginning_date_time, end_date_time)

# aggregate net exchange per grid area
from aggregation_utils.aggregators import NetExchangeGridAreaAggregator
aggregation_result_df = NetExchangeGridAreaAggregator.aggregate(valid_time_period_df)

# %%
aggregation_result_df \
    .write \
    .format("delta") \
    .save(OUTPUT_STORAGE_PATH)

# %%
