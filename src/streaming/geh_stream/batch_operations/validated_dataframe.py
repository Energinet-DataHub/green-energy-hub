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
from pyspark.sql import DataFrame, Window
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit
import pyspark.sql.functions as F

from geh_stream.monitoring import MonitoredStopwatch
import geh_stream.dataframelib as D


def add_time_series_validation_status_column(batch_df: DataFrame):
    return batch_df.withColumn("IsTimeSeriesValid", F.min(col("IsTimeSeriesPointValid")).over(Window.partitionBy("TimeSeries_mRID")))


def store_points_of_valid_time_series(batch_df: DataFrame, output_delta_lake_path, watch: MonitoredStopwatch):
    timer = watch.start_sub_timer(store_points_of_valid_time_series.__name__)
    batch_df \
        .filter(col("IsTimeSeriesValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("Period_Point_Time").alias("Time"),
                col("Period_Point_Quantity").alias("Quantity"),
                col("CorrelationId"),
                col("MessageReference"),
                col("MarketDocument_mRID"),
                col("MarketDocument_CreatedDateTime").alias("CreatedDateTime"),
                col("MarketDocument_SenderMarketParticipant_mRID").alias("SenderMarketParticipant_mRID"),
                col("MarketDocument_ProcessType").alias("ProcessType"),
                col("MarketDocument_SenderMarketParticipant_Type").alias("SenderMarketParticipantMarketRole_Type"),
                col("TimeSeries_mRID"),
                col("MktActivityRecord_Status"),
                col("MarketEvaluationPointType"),
                col("Period_Point_Quality").alias("Quality"),
                col("MeterReadingPeriodicity"),
                col("MeteringMethod"),
                col("MeteringGridArea_Domain_mRID"),
                col("ConnectionState"),
                col("EnergySupplier_MarketParticipant_mRID"),
                col("BalanceResponsibleParty_MarketParticipant_mRID"),
                col("InMeteringGridArea_Domain_mRID"),
                col("OutMeteringGridArea_Domain_mRID"),
                col("Parent_Domain_mRID"),
                col("MarketDocument_MarketServiceCategory_Kind").alias("ServiceCategory_Kind"),
                col("SettlementMethod"),
                col("QuantityMeasurementUnit_Name"),
                col("Product"),

                year("Period_Point_Time").alias("year"),
                month("Period_Point_Time").alias("month"),
                dayofmonth("Period_Point_Time").alias("day")) \
        .repartition("year", "month", "day") \
        .write \
        .partitionBy("year", "month", "day") \
        .format("delta") \
        .mode("append") \
        .save(output_delta_lake_path)
    timer.stop_timer()
