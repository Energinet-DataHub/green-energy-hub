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
"""
#
# Delta Lake Proof of Concept 
#
# The purpose of this POC is to demonstrate that it is possible to make a Delta Lake
# design, which support correction time series points. That is time series points in
# one time series, which is used to overwrite wrong time series points from previous
# received time series. A new correction time series point has the same
# Metering Point ID and Observation Time as tyhe old (wrong) time series point.
# 
# The POC uses a Delta Lake design where all received time series points are added to
# the Delta Lake database. Old time series points, which have been corrected, should
# then be filtered out, when retrieving data from the Delta Lake.
# In this design, time series point data in Delta Lake is never updated or deleted.
# They are only appended to the database. 
#

import configargparse
import pyspark
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import desc, col, row_number
from pyspark.sql.types import StructType, StructField, StringType, IntegerType, TimestampType
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
                  StructField("meteringPointId", IntegerType(), True),
                  StructField("timeSeriesId", IntegerType(), True),
                  StructField("timeSeriesPointId", IntegerType(), True),
                  StructField("observationDateTime", TimestampType(), True),
                  StructField("receivedDateTime", TimestampType(), True),
                  StructField("meteringValue", IntegerType(), True)
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
            df = self.spark.createDataFrame(data = timeSeries, schema = self.messageSchema)
            df.write \
                  .partitionBy("meteringPointId", "observationDateTime") \
                  .format("delta") \
                  .mode("append") \
                  .option("checkpointLocation", self.checkpoint_path) \
                  .save(self.delta_lake_path)

      def select_recent_values(self):
            windowSpec  = Window \
                  .partitionBy("meteringPointId", "observationDateTime") \
                  .orderBy(desc("receivedDateTime"))
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .withColumn("row_number", row_number().over(windowSpec)) \
                  .where(col("row_number") == 1)
            return df

      def select_values_reported_before_timestamp(self, timestamp):
            windowSpec  = Window \
                  .partitionBy("meteringPointId", "observationDateTime") \
                  .orderBy(desc("receivedDateTime"))
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .where(col("receivedDateTime") <= timestamp) \
                  .withColumn("row_number", row_number().over(windowSpec)) \
                  .where(col("row_number") == 1)
            return df

    
      def select_recent_values_for_metering_point(self, meteringPoint):
            windowSpec  = Window \
                  .partitionBy("meteringPointId", "observationDateTime") \
                  .orderBy(desc("receivedDateTime"))    
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .where(col("meteringPointId") == meteringPoint) \
                  .withColumn("row_number", row_number().over(windowSpec)) \
                  .where(col("row_number") == 1)
            return df

      def select_values_reported_for_metering_point_before_timestamp(self, meteringPoint, timestamp):
            windowSpec  = Window \
                  .partitionBy("meteringPointId", "observationDateTime") \
                  .orderBy(desc("receivedDateTime"))
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .where((col("meteringPointId") == meteringPoint) & (col("receivedDateTime") <= timestamp)) \
                  .withColumn("row_number", row_number().over(windowSpec)) \
                  .where(col("row_number") == 1)
            return df

      def select_history_for_metering_point(self, meteringPoint):
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .where(col("meteringPointId") == meteringPoint)
            return df

      def select_history_for_metering_point_observation_time(self, meteringPoint, observationDateTime):
            df = self.spark.read.format("delta") \
                  .load(self.delta_lake_path) \
                  .where((col("meteringPointId") == meteringPoint) & (col("observationDateTime") == observationDateTime))
            return df
      
      def get_all_content(self):
            df = self.spark.read.format("delta").load(self.delta_lake_path)
            return df
