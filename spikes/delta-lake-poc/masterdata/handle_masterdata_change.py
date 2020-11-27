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
Delta Lake POC for masterdata change back in time

The purpose of this POC is to demonstrate how time series points already
stored in Delta Lake can be updated following a master data change
back in time.

This class has the following capabilities:
    - Fetching all time series point for a metering point since a timestamp.
    - Fetching the currently valid time series points for a metering point since a timestamp.
    - Change the supplier (from the old to the new) for a given dataset, the "from-validity" of each time series point is updated.
Time series point data in Delta Lake is never updated,they are only
appended to the database.
"""

import configargparse
import pyspark
from pyspark import SparkConf
from pyspark.sql import SparkSession
from pyspark.sql.functions import year, month, dayofmonth, desc, col, row_number, lit, UserDefinedFunction
from pyspark.sql.types import StringType, TimestampType, DecimalType, StructType, DoubleType
from pyspark.sql.window import Window
from datetime import datetime
from decimal import Decimal


class HandleMasterdataChange:

    @staticmethod
    def read_configuration():
            p = configargparse.ArgParser(prog='poc_handle_masterdata_update_new_supplier.py',
                description='Green Energy Hub Streaming',
                default_config_files=['configuration/run_args_poc_handle_masterdata_update_new_supplier.conf'],
                formatter_class=configargparse.ArgumentDefaultsHelpFormatter)
            p.add('--storage-account-name', type=str, required=True,
                help='Azure Storage account name (used for data output and checkpointing)')
            p.add('--storage-account-key', type=str, required=True,
                help='Azure Storage key', env_var='GEH_STREAMING_STORAGE_KEY')
            p.add('--storage-container-name', type=str, required=False, default='data',
                help='Azure Storage container name')
            p.add('--timeseries-path', type=str, required=False, default="delta/meter-data/",
                help='Path to timeseries storage location (deltalake) relative to container''s root')
            p.add('--checkpoint-path', type=str, required=False, default="delta/checkpoint/",
                help='Path to checkpoints location (deltalake)')

            args, unknown_args = p.parse_known_args()
            return args

    def __init__(self):
        args = HandleMasterdataChange.read_configuration()
        storage_account_name = args.storage_account_name
        storage_account_key = args.storage_account_key
        container_name = args.storage_container_name
        timeseries_path = args.timeseries_path
        checkpoint_path = args.checkpoint_path

        self.delta_lake_path = "wasbs://" + container_name + "@" + storage_account_name + ".blob.core.windows.net/" + timeseries_path
        self.checkpoint_path = "wasbs://" + container_name + "@" + storage_account_name + ".blob.core.windows.net/" + checkpoint_path

        spark_conf = SparkConf(loadDefaults=True) \
            .set('fs.azure.account.key.{0}.blob.core.windows.net'.format(storage_account_name), storage_account_key) \
            .set("spark.databricks.delta.preview.enabled", "true")

        self.spark = SparkSession\
            .builder\
            .config(conf=spark_conf)\
            .getOrCreate()

    def get_time_series_point_for_metering_point_since_timestamp(self, metering_point_id, start_date_time):
        timeseries_df = self.spark.read.format("delta") \
            .load(self.delta_lake_path) \
            .where((col("MarketDocument_mRID") == metering_point_id) & (col("ObservationTime") >= start_date_time))

        return timeseries_df

    def get_current_valid_time_series_point_for_metering_point_since_timestamp(self, metering_point_id, start_date_time):
        timeseries_df_since_date = self.get_time_series_point_for_metering_point_since_timestamp(metering_point_id, start_date_time)
        windowSpec = Window.partitionBy("ObservationTime").orderBy(desc("ValidFrom"))
        df_to_return = timeseries_df_since_date \
            .withColumn("row_number", row_number().over(windowSpec)) \
            .where(col("row_number") == 1) \
            .orderBy("ObservationTime")

        return df_to_return

    def change_supplier_of_time_series_point(self, timeseries_points_df, old_supplier, new_supplier):
        timeseries_withOldSupplier_df = timeseries_points_df \
            .where(col("EnergySupplier_MarketParticipant_mRID") == old_supplier)

        timeseries_withNewSupplier_df = timeseries_withOldSupplier_df \
            .withColumn("EnergySupplier_MarketParticipant_mRID", lit(new_supplier)) \
            .withColumn("ValidFrom", lit(datetime.now()))

        return timeseries_withNewSupplier_df

    def add_time_series_to_delta_lake(self, timeseries_df):
        timeseries_df = self.get_columns_from_time_series_point_df_to_store(timeseries_df) \
                            .repartition("MeteringGridArea_Domain_mRID", "year", "month", "day") \
                            .write \
                            .partitionBy("MeteringGridArea_Domain_mRID", "year", "month", "day") \
                            .format("delta") \
                            .mode("append") \
                            .save(self.delta_lake_path)

    def add_test_data_time_series_to_delta_lake(self, timeseries):
        timeseries_df = self.spark.createDataFrame(timeseries, self.test_data_schema)
        self.add_time_series_to_delta_lake(timeseries_df)

    def get_columns_from_time_series_point_df_to_store(self, timeseries):
        return timeseries.select(
            col("CorrelationId"),
            col("MessageReference"),
            col("MarketDocument_mRID"),
            col("CreatedDateTime"),
            col("SenderMarketParticipant_mRID"),
            col("ProcessType"),
            col("SenderMarketParticipantMarketRoleType"),
            col("MarketServiceCategoryKind"),
            col("TimeSeries_mRID"),
            col("MktActivityRecordStatus"),
            col("Product"),
            col("UnitName"),
            col("MarketEvaluationPointType"),
            col("SettlementMethod"),
            col("MarketEvaluationPoint_mRID"),
            col("Quantity"),
            col("Quality"),
            col("ObservationTime"),
            col("MeteringMethod"),
            col("MeterReadingPeriodicity"),
            col("MeteringGridArea_Domain_mRID"),
            col("ConnectionState"),
            col("EnergySupplier_MarketParticipant_mRID"),
            col("BalanceResponsibleParty_MarketParticipant_mRID"),
            col("InMeteringGridArea_Domain_mRID"),
            col("OutMeteringGridArea_Domain_mRID"),
            col("Parent_Domain"),
            col("ServiceCategoryKind"),
            col("Technology"),
            col("ValidFrom"),
            year("ObservationTime").alias("year"),
            month("ObservationTime").alias("month"),
            dayofmonth("ObservationTime").alias("day"))

    def create_test_element(self, observation_time, supplier_id, valid_from):
        return ("a", "b", "MP1", datetime(1970, 1, 1, 0, 0, 0), "e", "f", "g", "h", "tsId1", "i", "ii", "j", "k", "l", "m", Decimal(1), "n", observation_time, "o", "p", "GridAreaA", "q", supplier_id, "balResponA", "r", "s", "t", "u", "v", valid_from)

    test_data_schema: StructType = StructType() \
        .add("CorrelationId", StringType(), False) \
        .add("MessageReference", StringType(), False) \
        .add("MarketDocument_mRID", StringType(), False) \
        .add("CreatedDateTime", TimestampType(), False) \
        .add("SenderMarketParticipant_mRID", StringType(), False) \
        .add("ProcessType", StringType(), False) \
        .add("SenderMarketParticipantMarketRoleType", StringType(), False) \
        .add("MarketServiceCategoryKind", StringType(), False) \
        .add("TimeSeries_mRID", StringType(), False) \
        .add("MktActivityRecordStatus", StringType(), False) \
        .add("Product", StringType(), False) \
        .add("UnitName", StringType(), False) \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType(), True) \
        .add("MarketEvaluationPoint_mRID", StringType(), False) \
        .add("Quantity", DecimalType(), True) \
        .add("Quality", StringType(), True) \
        .add("ObservationTime", TimestampType(), False) \
        .add("MeteringMethod", StringType(), True) \
        .add("MeterReadingPeriodicity", StringType(), True) \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("ConnectionState", StringType(), False) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType(), True) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType(), True) \
        .add("InMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("OutMeteringGridArea_Domain_mRID", StringType(), True) \
        .add("Parent_Domain", StringType(), True) \
        .add("ServiceCategoryKind", StringType(), True) \
        .add("Technology", StringType(), True) \
        .add("ValidFrom", TimestampType(), False)
