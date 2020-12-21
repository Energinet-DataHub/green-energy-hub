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
import inspect
from pyspark.sql import DataFrame, Window
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit
import pyspark.sql.functions as F

import geh_stream.dataframelib as D


def add_time_series_validation_status_column(batch_df: DataFrame):
    return batch_df.withColumn("IsTimeSeriesValid", F.min(col("IsTimeSeriesPointValid")).over(Window.partitionBy("TimeSeries_mRID")))


def send_valid_data(batch_df: DataFrame, output_eh_conf, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsTimeSeriesValid") == lit(True)) \
        .withColumn("Point",
                    struct(col("Period_Point_ObservationTime").alias("ObservationTime"),
                           col("Period_Point_Quality").alias("Quality"),
                           col("Period_Point_Quantity").alias("Quantity"))) \
        .groupBy("TimeSeries_mRID") \
        .agg(D.first("TimeSeries_mRID"),
             D.first(col("MarketEvaluationPoint_mRID")).alias("MarketEvaluationPoint_mRID"),
             D.first(col("CorrelationId")),
             D.first(col("MessageReference")),
             D.first(col("MarketDocument_mRID")).alias("MarketDocument_mRID"),
             D.first(col("MarketDocument_CreatedDateTime")).alias("CreatedDateTime"),
             D.first(col("MarketDocument_SenderMarketParticipant_mRID")).alias("SenderMarketParticipant_mRID"),
             D.first(col("MarketDocument_ProcessType")).alias("ProcessType"),
             D.first(col("MarketDocument_SenderMarketParticipant_Type")).alias("SenderMarketParticipantMarketRole_Type"),
             D.first(col("TimeSeries_mRID")),
             D.first(col("MktActivityRecord_Status")),
             D.first(col("MarketEvaluationPointType")),
             D.first(col("MeterReadingPeriodicity")),
             D.first(col("MeteringMethod")),
             D.first(col("MeteringGridArea_Domain_mRID")),
             D.first(col("ConnectionState")),
             D.first(col("EnergySupplier_MarketParticipant_mRID")),
             D.first(col("BalanceResponsibleParty_MarketParticipant_mRID")),
             D.first(col("InMeteringGridArea_Domain_mRID")),
             D.first(col("OutMeteringGridArea_Domain_mRID")),
             D.first(col("Parent_Domain_mRID")),
             D.first(col("ServiceCategory_Kind")),
             D.first(col("SettlementMethod")),
             D.first(col("QuantityMeasurementUnit_Name")),
             D.first(col("Product")),
             D.first(col("RecipientList")),
             F.collect_list("Point").alias("Points")) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_eh_conf) \
        .save()
    timer.stop_timer()


def send_invalid_data(batch_df: DataFrame, output_invalid_eh_conf, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsTimeSeriesValid") == lit(False)) \
        .groupBy("TimeSeries_mRID") \
        .agg(D.first("TimeSeries_mRID"),
             D.first(col("MarketDocument_ProcessType")).alias("ProcessType"),
             D.first(col("MarketDocument_SenderMarketParticipant_Type")).alias("SenderMarketParticipantMarketRole_Type"),
             D.first(col("MarketDocument_SenderMarketParticipant_mRID")).alias("SenderMarketParticipant_mRID"),
             D.first(col("MarketDocument_mRID")),
             D.first(col("MktActivityRecord_Status")),
             D.min(col("VR-200-Is-Valid")),
             D.min(col("VR-245-1-Is-Valid")),
             D.min(col("VR-250-Is-Valid")),
             D.min(col("VR-251-Is-Valid")),
             D.min(col("VR-611-Is-Valid")),
             D.min(col("VR-612-Is-Valid"))) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_invalid_eh_conf) \
        .save()
    timer.stop_timer()


def store_valid_data(batch_df: DataFrame, output_delta_lake_path, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsTimeSeriesValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("Period_Point_ObservationTime").alias("ObservationTime"),
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

                year("Period_Point_ObservationTime").alias("year"),
                month("Period_Point_ObservationTime").alias("month"),
                dayofmonth("Period_Point_ObservationTime").alias("day")) \
        .repartition("year", "month", "day") \
        .write \
        .partitionBy("year", "month", "day") \
        .format("delta") \
        .mode("append") \
        .save(output_delta_lake_path)
    timer.stop_timer()
