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
from pyspark.sql import DataFrame
from pyspark.sql.types import StructType, TimestampType, StringType, DoubleType
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit


def send_valid_data(batch_df: DataFrame, output_eh_conf, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("ObservationTime"),
                col("Quantity"),
                col("CorrelationId"),
                col("MessageReference"),
                col("MarketDocument_mRID"),
                col("CreatedDateTime"),
                col("SenderMarketParticipant_mRID"),
                col("ProcessType"),
                col("SenderMarketParticipantMarketRole_Type"),
                col("TimeSeries_mRID"),
                col("MktActivityRecord_Status"),
                col("MarketEvaluationPointType"),
                col("Quality"),
                col("MeterReadingPeriodicity"),
                col("MeteringMethod"),
                col("MeteringGridArea_Domain_mRID"),
                col("ConnectionState"),
                col("EnergySupplier_MarketParticipant_mRID"),
                col("BalanceResponsibleParty_MarketParticipant_mRID"),
                col("InMeteringGridArea_Domain_mRID"),
                col("OutMeteringGridArea_Domain_mRID"),
                col("Parent_Domain_mRID"),
                col("ServiceCategory_Kind"),
                col("SettlementMethod"),
                col("QuantityMeasurementUnit_Name"),
                col("Product"),
                col("ObservationTime")) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_eh_conf) \
        .save()
    timer.stop_timer()


def send_invalid_data(batch_df: DataFrame, output_invalid_eh_conf, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsValid") == lit(False)) \
        .select(col("ProcessType"),
                col("SenderMarketParticipantMarketRole_Type"),
                col("SenderMarketParticipant_mRID"),
                col("MarketDocument_mRID"),
                col("MktActivityRecord_Status"),
                col("VR-245-1-Is-Valid"),
                col("VR-250-Is-Valid"),
                col("VR-251-Is-Valid"),
                col("VR-611-Is-Valid"),
                col("VR-612-Is-Valid")) \
        .select(to_json(struct(col("*"))).cast("string").alias("body")) \
        .write \
        .format("eventhubs") \
        .options(**output_invalid_eh_conf) \
        .save()
    timer.stop_timer()


def store_valid_data(batch_df: DataFrame, output_delta_lake_path, watch):
    timer = watch.start_sub_timer(inspect.currentframe().f_code.co_name)
    batch_df \
        .filter(col("IsValid") == lit(True)) \
        .select(col("MarketEvaluationPoint_mRID"),
                col("ObservationTime"),
                col("Quantity"),
                col("CorrelationId"),
                col("MessageReference"),
                col("MarketDocument_mRID"),
                col("CreatedDateTime"),
                col("SenderMarketParticipant_mRID"),
                col("ProcessType"),
                col("SenderMarketParticipantMarketRole_Type"),
                col("TimeSeries_mRID"),
                col("MktActivityRecord_Status"),
                col("MarketEvaluationPointType"),
                col("Quality"),
                col("MeterReadingPeriodicity"),
                col("MeteringMethod"),
                col("MeteringGridArea_Domain_mRID"),
                col("ConnectionState"),
                col("EnergySupplier_MarketParticipant_mRID"),
                col("BalanceResponsibleParty_MarketParticipant_mRID"),
                col("InMeteringGridArea_Domain_mRID"),
                col("OutMeteringGridArea_Domain_mRID"),
                col("Parent_Domain_mRID"),
                col("ServiceCategory_Kind"),
                col("SettlementMethod"),
                col("QuantityMeasurementUnit_Name"),
                col("Product"),

                year("ObservationTime").alias("year"),
                month("ObservationTime").alias("month"),
                dayofmonth("ObservationTime").alias("day")) \
        .repartition("year", "month", "day") \
        .write \
        .partitionBy("year", "month", "day") \
        .format("delta") \
        .mode("append") \
        .save(output_delta_lake_path)
    timer.stop_timer()
