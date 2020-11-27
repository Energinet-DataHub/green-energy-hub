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

The purpose of this POC is to demonstrate that it is possible to make a Delta Lake
design, which support correction time series points. That is time series points in
one time series, which is used to overwrite wrong time series points from previous
received time series. A new correction time series point has the same
Metering Point ID and Observation Time as the old (wrong) time series point.

The POC uses a Delta Lake design where all received time series points are added to
the Delta Lake database. Old time series points, which have been corrected, should
then be filtered out, when retrieving data from the Delta Lake.
In this design, time series point data in Delta Lake is never updated or deleted.
They are only appended to the database.

The POC also demonstrates, that it is possible to handle soft deletes in Delta Lake.
When a values is soft deleted, it will be visible when selecting recent values but
it will still be possible to find the deleted value when looking for historical data.

The main strategy for soft delete is to use pseudo time series values marked with a
"Soft Delete" state. These pseudo values will shadow the real soft deleted values
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
"""

import configargparse
import pyspark
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import desc, col, row_number, when, udf
from pyspark.sql.types import StructType, StructField, StringType, IntegerType, TimestampType, BooleanType
from pyspark.sql.window import Window
from datetime import datetime


class DeltaLakePOC:

    @staticmethod
    def read_configuration():
        p = configargparse.ArgParser(prog='corrections.py',
                                     description='Green Energy Hub Streaming',
                                     default_config_files=['configuration/run_args_corrections.conf'],
                                     formatter_class=configargparse.ArgumentDefaultsHelpFormatter)
        p.add('--storage-account-name', type=str, required=False,
              help='Azure Storage account name (used for data output and checkpointing)')
        p.add('--storage-account-key', type=str, required=False,
              help='Azure Storage key', env_var='GEH_STREAMING_STORAGE_KEY')
        p.add('--storage-container-name', type=str, required=False, default='data',
              help='Azure Storage container name')

        args, unknown_args = p.parse_known_args()
        return args

    @staticmethod
    def setup_message_schema():
        messageSchema = StructType([
            StructField("meteringPointId", StringType(), True),
            StructField("timeSeriesId", StringType(), True),
            StructField("timeSeriesPointId", StringType(), True),
            StructField("observationDateTime", TimestampType(), True),
            StructField("receivedDateTime", TimestampType(), True),
            StructField("meteringValue", IntegerType(), True),
            StructField("quality", StringType(), True)
        ])
        return messageSchema

    def __init__(self):
        args = DeltaLakePOC.read_configuration()
        storage_account_name = args.storage_account_name
        storage_account_key = args.storage_account_key
        container_name = args.storage_container_name

        self.delta_lake_path = "wasbs://" + container_name + "@" + storage_account_name + ".blob.core.windows.net/messages/"
        self.checkpoint_path = "wasbs://" + container_name + "@" + storage_account_name + ".blob.core.windows.net/checkpoint/"

        self.messageSchema = DeltaLakePOC.setup_message_schema()

        spark_conf = SparkConf(loadDefaults=True) \
            .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(storage_account_name), storage_account_key) \
            .set("spark.databricks.delta.preview.enabled", "true")

        self.spark = SparkSession\
            .builder\
            .config(conf=spark_conf)\
            .getOrCreate()

    def add_time_series_to_delta_lake(self, timeSeries):
        df = self.spark.createDataFrame(data=timeSeries, schema=self.messageSchema)
        df.write \
            .partitionBy("meteringPointId", "observationDateTime") \
            .format("delta") \
            .mode("append") \
            .option("checkpointLocation", self.checkpoint_path) \
            .save(self.delta_lake_path)

    def soft_delete(self, meteringPoint, observationTime, deleteTimestamp):
        data = [(meteringPoint, "", "", observationTime, deleteTimestamp, 0, "Soft Delete")]
        df = self.spark.createDataFrame(data=data, schema=self.messageSchema)
        df.write \
            .partitionBy("meteringPointId", "observationDateTime") \
            .format("delta") \
            .mode("append") \
            .option("checkpointLocation", self.checkpoint_path) \
            .save(self.delta_lake_path)

    def soft_delete_period(self, meteringPoint, startObservationDateTime, endObservationDateTime, deleteTimestamp):
        df = self.select_values_for_metering_point_observation_time_period_before_timestamp(
            meteringPoint, startObservationDateTime, endObservationDateTime, deleteTimestamp)
        points = df.collect()
        for p in points:
            self.soft_delete(p.meteringPointId, p.observationDateTime, deleteTimestamp)

    def select_recent_values(self):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where((col("row_number") == 1) & (col("quality") != "Soft Delete")) \
            .orderBy("meteringPointId", "observationDateTime")
        return df.drop("row_number")

    def select_recent_values_with_soft_delete_values(self):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where(col("row_number") == 1) \
            .orderBy("meteringPointId", "observationDateTime")
        return df.drop("row_number")

    def select_values_before_timestamp(self, timestamp):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where(col("receivedDateTime") <= timestamp) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where((col("row_number") == 1) & (col("quality") != "Soft Delete")) \
            .orderBy("meteringPointId", "observationDateTime")
        return df.drop("row_number")

    def select_values_for_metering_point_time_series(self, meteringPoint, timeSeries):
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint) & (col("timeSeriesId") == timeSeries)) \
            .orderBy("observationDateTime")
        return df

    def select_recent_values_for_metering_point(self, meteringPoint):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where(col("meteringPointId") == meteringPoint) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where((col("row_number") == 1) & (col("quality") != "Soft Delete")) \
            .orderBy("observationDateTime")
        return df.drop("row_number")

    def select_values_for_metering_point_before_timestamp(self, meteringPoint, timestamp):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint) & (col("receivedDateTime") <= timestamp)) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where((col("row_number") == 1) & (col("quality") != "Soft Delete")) \
            .orderBy("observationDateTime")
        return df.drop("row_number")

    def select_values_for_metering_point_observation_time_period_before_timestamp(
            self, meteringPoint, startObservationDateTime, endObservationDateTime, timestamp):
        windowSpec = Window \
            .partitionBy("meteringPointId", "observationDateTime") \
            .orderBy(desc("receivedDateTime"))
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint)
                   & (col("observationDateTime") >= startObservationDateTime)
                   & (col("observationDateTime") <= endObservationDateTime)
                   & (col("receivedDateTime") <= timestamp)) \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where((col("row_number") == 1) & (col("quality") != "Soft Delete")) \
            .orderBy("observationDateTime")
        return df.drop("row_number")

    def select_all_history_for_metering_point(self, meteringPoint):
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where(col("meteringPointId") == meteringPoint) \
            .orderBy("receivedDateTime", "observationDateTime")
        return df

    def find_soft_deletes(self, df):
        soft_deletes = df.filter(col("quality") == "Soft Delete").collect()
        return soft_deletes

    def find_last_soft_delete(self, df):
        soft_deletes = self.find_soft_deletes(df)
        if not soft_deletes:
            return (False, None)
        last_deleted_time = soft_deletes[len(soft_deletes) - 1].receivedDateTime
        return (True, last_deleted_time)

    @staticmethod
    def is_soft_deleted(observationDateTime, receivedDateTime, soft_deletes):
        for sd in soft_deletes:
            if (observationDateTime == sd.observationDateTime) & (receivedDateTime < sd.receivedDateTime):
                return True
        return False

    def filter_out_soft_deletes(self, df1):
        soft_deletes = self.find_soft_deletes(df1)
        if not soft_deletes:
            return df1
        filter_udf = udf(lambda x, y: not DeltaLakePOC.is_soft_deleted(x, y, soft_deletes), BooleanType())
        df2 = df1.filter(df1.quality != "Soft Delete").filter(filter_udf(df1.observationDateTime, df1.receivedDateTime))
        return df2

    def select_history_for_metering_point(self, meteringPoint):
        df1 = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where(col("meteringPointId") == meteringPoint) \
            .orderBy("receivedDateTime", "observationDateTime")
        df2 = self.filter_out_soft_deletes(df1)
        return df2

    @staticmethod
    def find_newer_time_series_point(points, meteringPoint, observationDateTime, receivedDateTime):
        first_update = None
        for p in points:
            if (p.meteringPointId == meteringPoint) & (p.observationDateTime == observationDateTime):
                if p.receivedDateTime > receivedDateTime:
                    if p.quality == "Soft Delete":
                        return p
                    if not first_update:
                        first_update = p
        return first_update

    @staticmethod
    def calculate_state(points, meteringPoint, observationDateTime, receivedDateTime):
        p = DeltaLakePOC.find_newer_time_series_point(points, meteringPoint, observationDateTime, receivedDateTime)
        if p:
            if p.quality == "Soft Delete":
                return "Deleted"
            else:
                return "Updated"
        return " "

    def process_soft_deletes(self, df1):
        points = df1.collect()
        status_udf = udf(lambda mp, obs, rec: DeltaLakePOC.calculate_state(points, mp, obs, rec), StringType())
        df2 = df1.withColumn("state", status_udf(col("meteringPointId"), col("observationDateTime"), col("receivedDateTime")))
        return df2

    def select_processed_history_for_metering_point(self, meteringPoint):
        df1 = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where(col("meteringPointId") == meteringPoint) \
            .orderBy("receivedDateTime", "observationDateTime")
        df2 = self.process_soft_deletes(df1)
        return df2

    def select_all_history_for_metering_point_observation_time(self, meteringPoint, observationDateTime):
        df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint) & (col("observationDateTime") == observationDateTime)) \
            .orderBy("receivedDateTime")
        return df

    def select_history_for_metering_point_observation_time(self, meteringPoint, observationDateTime):
        df1 = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint) & (col("observationDateTime") == observationDateTime)) \
            .orderBy("receivedDateTime")
        df2 = self.filter_out_soft_deletes(df1)
        return df2

    def select_processed_history_for_metering_point_observation_time(self, meteringPoint, observationDateTime):
        df1 = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("meteringPointId") == meteringPoint) & (col("observationDateTime") == observationDateTime)) \
            .orderBy("receivedDateTime")
        df2 = self.process_soft_deletes(df1)
        return df2

    def get_all_content(self):
        df = self.spark.read.format("delta").load(self.delta_lake_path) \
            .orderBy("receivedDateTime", "observationDateTime")
        return df
