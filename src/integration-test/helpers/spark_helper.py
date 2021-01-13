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
from pyspark import SparkConf
from pyspark.sql import SparkSession
from pyspark.sql.functions import array, array_except, when, col, lit
from pyspark.sql.utils import AnalysisException
import time
import datetime
from datetime import timedelta


def get_base_storage_path(storage_container_name, storage_account_name):
    return "abfss://{0}@{1}.dfs.core.windows.net/".format(
        storage_container_name, storage_account_name)


def get_spark_session(storage_account_name, storage_account_key):
    spark_conf = SparkConf(loadDefaults=True) \
        .set('fs.azure.account.key.{0}.dfs.core.windows.net'.format(storage_account_name), storage_account_key)

    spark = spark = SparkSession\
        .builder\
        .config(conf=spark_conf)\
        .getOrCreate()

    sc = spark.sparkContext
    print("Spark Configuration:")
    _ = [print(k + '=' + v) for k, v in sc.getConf().getAll()]

    return spark


def find_row_in_delta_lake(spark, directory, column_name, column_value):
    # Handle exception that occurs if necessary Delta directory hasn't been created by the streaming yet
    # and no other or previous test runs have created it.
    try:
        df = spark.read.format("delta").load(directory)
    except AnalysisException:
        return None

    filter_result = df.filter(col(column_name) == column_value)

    if filter_result.count() == 0:
        return None

    return filter_result.first()


def get_stored_value_in_deltalake(spark, function_timeout_minutes, directory, column_name, column_value):
    sleep_time_in_seconds = 1
    wait_until = datetime.datetime.now() + timedelta(minutes=function_timeout_minutes)
    break_loop = False
    while not break_loop:
        file = find_row_in_delta_lake(spark, directory, column_name, column_value)

        if file is not None:
            return file

        time.sleep(sleep_time_in_seconds)
        if wait_until < datetime.datetime.now():
            break_loop = True

    return None
