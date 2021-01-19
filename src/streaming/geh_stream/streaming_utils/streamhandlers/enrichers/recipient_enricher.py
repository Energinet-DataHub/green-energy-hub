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

from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.functions import array, array_except, when, col, lit
from geh_stream.codelists.market_evaluation_point_type import MarketEvaluationPointType

# Danish Energy Agency mrid
sts_mrid = "5790001330584"
# System Operator mrid
ez_mrid = "5790000432752"


def enrich_recipients(enriched_data: DataFrame):
    recipient_enriched_data = enriched_data
    recipient_enriched_data = find_sts_recipient(recipient_enriched_data)
    recipient_enriched_data = find_ddq_recipient(recipient_enriched_data)
    recipient_enriched_data = find_ez_recipient(recipient_enriched_data)
    recipient_enriched_data = find_ddm_recipients(recipient_enriched_data)
    recipient_enriched_data = merge_intermediate_recipient_columns(recipient_enriched_data)
    recipient_enriched_data = remove_null_values_from_recipient_column_array(recipient_enriched_data)
    recipient_enriched_data = drop_intermediate_recipient_columns(recipient_enriched_data)
    return recipient_enriched_data


def find_sts_recipient(enriched_data: DataFrame):
    return enriched_data.withColumn(
        "intermediate_sts_recipient",
        when((col("md.MarketEvaluationPointType") == MarketEvaluationPointType.production.value)
             | (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.ve_production.value)
             | (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.surplus_production_group.value),
             sts_mrid)
        .otherwise(None))


def find_ddq_recipient(enriched_data: DataFrame):
    return enriched_data.withColumn("intermediate_ddq_recipient",
                                    when(col("md.EnergySupplier_MarketParticipant_mRID")
                                         .isNotNull(),
                                         col("md.EnergySupplier_MarketParticipant_mRID"))
                                    .otherwise(None))


def find_ez_recipient(enriched_data: DataFrame):
    return enriched_data.withColumn(
        "intermediate_ez_recipient",
        when(col("md.MarketEvaluationPointType") == MarketEvaluationPointType.analysis.value, ez_mrid)
        .otherwise(None))


def find_ddm_recipients(enriched_data: DataFrame):
    in_column = enriched_data.withColumn(
        "intermediate_ddm_in_recipient",
        when(((col("md.MarketEvaluationPointType") == MarketEvaluationPointType.exchange.value)
             | (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.reactive_energy.value))
             & (col("md.MeteringGridArea_Domain_mRID") != col("md.InMeteringGridArea_Domain_mRID")),
             col("md.InMeteringGridArea_Domain_Owner_mRID"))
        .otherwise(None))

    both_columns = in_column.withColumn(
        "intermediate_ddm_out_recipient",
        when(((col("md.MarketEvaluationPointType") == MarketEvaluationPointType.exchange.value)
             | (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.reactive_energy.value))
             & (col("md.MeteringGridArea_Domain_mRID") != col("md.OutMeteringGridArea_Domain_mRID")),
             col("md.OutMeteringGridArea_Domain_Owner_mRID"))
        .otherwise(None))

    return both_columns


def merge_intermediate_recipient_columns(enriched_data: DataFrame):
    return enriched_data.withColumn("recipient_list_with_nones",
                                    array(col("intermediate_sts_recipient"),
                                          col("intermediate_ddq_recipient"),
                                          col("intermediate_ddm_in_recipient"),
                                          col("intermediate_ddm_out_recipient"),
                                          col("intermediate_ez_recipient")))


def remove_null_values_from_recipient_column_array(enriched_data: DataFrame):
    return enriched_data.withColumn("RecipientList",
                                    array_except(
                                        col("recipient_list_with_nones"),
                                        array(lit(None).cast('string'))))


def drop_intermediate_recipient_columns(enriched_data: DataFrame):
    return enriched_data.drop(col("intermediate_sts_recipient")) \
        .drop(col("intermediate_ddq_recipient")) \
        .drop(col("intermediate_ddm_in_recipient")) \
        .drop(col("intermediate_ddm_out_recipient")) \
        .drop(col("intermediate_ez_recipient")) \
        .drop(col("recipient_list_with_nones"))
