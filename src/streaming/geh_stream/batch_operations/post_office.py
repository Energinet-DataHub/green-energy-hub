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

from pyspark.sql import DataFrame
from pyspark.sql.functions import year, month, dayofmonth, to_json, \
    struct, col, from_json, coalesce, lit, explode, date_format, concat, \
    when, array, array_except
import geh_stream.dataframelib as D
import pyspark.sql.functions as F


class PostOffice:
    """
    Extracts relevant information from enriched and validated streaming dataframe
    to send to market actors. Send to intermediate cache storage for pick up.
    """

    __dateFormat = "yyyy-MM-dd'T'HH:mm:ssX"

    def __init__(self, write_config=None):
        self.cosmos_write_config = write_config

    def sendToCosmosDb(self, df: DataFrame, numPartitions: int):
        # Issues with CosmosDB writing performance were due to the design of the adapter. In the bulk mode optimized
        # for batch processing it waits for a few seconds if the amount of incoming messages to write is lower than
        # some predefined threshold. But this never happens in forEachBatch loop, so that we were just waiting for
        # every of 200 partitions.
        #
        # Current fix partially overcomes this behavior repartitioning the dataframe before write to the maximum
        # parallelism level of the job. With the bigger partitions under the average load it should start writing
        # immediately, but even if not, it will at least wait in parallel.
        #
        # Consider switching to transactional upload when it's available (April 2021?) as it doesn't have waits, but mind
        # loading overhead of loading with every row as a transaction.
        df \
            .repartition(numPartitions) \
            .write \
            .format("com.microsoft.azure.cosmosdb.spark") \
            .options(**self.cosmos_write_config) \
            .mode("append") \
            .save()

    def extractValidMessageAtomicValues(self, batch_df: DataFrame):
        # explode destroys rows with empty RecipientList array
        df = batch_df \
            .filter(col("IsTimeSeriesValid") == lit(True)) \
            .select(lit("ValidObservation").alias("MessageType"),
                    col("CorrelationId"),
                    col("MarketEvaluationPoint_mRID"),
                    col("MeterReadingPeriodicity"),
                    col("Product"),
                    col("QuantityMeasurementUnit_Name"),
                    col("MarketEvaluationPointType"),
                    col("SettlementMethod"),
                    col("MarketDocument_ProcessType").alias("ProcessType"),
                    col("MarketDocument_RecipientMarketParticipant_Type").alias("RecipientMarketParticipantMarketRole_Type"),
                    col("MarketDocument_MarketServiceCategory_Kind").alias("MarketServiceCategory_Kind"),
                    explode("RecipientList").alias("RecipientMarketParticipant_mRID"),
                    col("Period_Point_Quantity").alias("Quantity"),
                    col("Period_Point_Quality").alias("Quality"),
                    date_format("EventHubEnqueueTime", self.__dateFormat).alias("EventHubEnqueueTime"),
                    date_format("Period_Point_Time", self.__dateFormat).alias("ObservationTime"),
                    date_format("MarketDocument_CreatedDateTime", self.__dateFormat).alias("MarketDocument_CreatedDateTime")) \
            .filter(col("RecipientMarketParticipant_mRID").isNotNull())
        return df

    def extractInvalidMessageAtomicValues(self, batch_df: DataFrame):
        vr_cols = [c for c in batch_df.columns if c.startswith("VR-")]
        min_vr_cols = [D.min(c) for c in vr_cols]

        df = batch_df \
            .filter(col("IsTimeSeriesValid") == lit(False)) \
            .filter(col("MarketDocument_SenderMarketParticipant_mRID").isNotNull()) \
            .groupBy("TimeSeries_mRID") \
            .agg(lit("InvalidTimeSeries").alias("MessageType"),
                 D.first("CorrelationId"),
                 F.first("MarketDocument_SenderMarketParticipant_mRID").alias("RecipientMarketParticipant_mRID"),
                 F.first("MarketDocument_SenderMarketParticipant_Type").alias("RecipientMarketParticipantMarketRole_Type"),
                 D.first("MarketDocument_mRID"),
                 F.first("MarketDocument_ProcessType").alias("ProcessType"),
                 *min_vr_cols) \
            .filter(col("RecipientMarketParticipant_mRID").isNotNull())

        reasoned_df = self.extractReasons(df) \
            .drop(*vr_cols)

        return reasoned_df

    def sendValid(self, batch_df: DataFrame, numPartitions: int):
        df = self.extractValidMessageAtomicValues(batch_df)
        self.sendToCosmosDb(df, numPartitions)

    def sendRejected(self, batch_df: DataFrame, numPartitions: int):
        df = self.extractInvalidMessageAtomicValues(batch_df)
        self.sendToCosmosDb(df, numPartitions)

    def extractReasons(self, batch_df: DataFrame):
        vr_cols = [c for c in batch_df.columns if c.startswith("VR-")]

        literal_vr_df = batch_df
        for col_name in vr_cols:
            literal_vr_df = literal_vr_df \
                .withColumn(col_name + "__Temp", when(~col(col_name), struct(
                    lit(col_name.replace("-Is-Valid", "")).alias("Reason"),
                    lit("").alias("Reason_Text"))
                ).otherwise(lit(None)))

        temp_vr_columns = [c + "__Temp" for c in vr_cols]
        reasoned_vr_df = literal_vr_df \
            .withColumn("Reasons", array(temp_vr_columns)) \
            .withColumn("Reasons", array_except(col("Reasons"), array(lit(None)))) \
            .drop(*temp_vr_columns)

        return reasoned_vr_df
