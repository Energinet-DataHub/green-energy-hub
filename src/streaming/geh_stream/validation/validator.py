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
from pyspark.sql.functions import col
import pyspark.sql.functions as F

from .rules import rules


class Validator:

    @staticmethod
    def add_validation_status_columns(enriched_data: DataFrame):
        validated_data = enriched_data

        for rule in rules:
            validated_data = rule(validated_data)

        # Dropped columns are duplicates (from both streamed data and master data).
        # They should not be necessary after validation.
        return validated_data \
            .withColumn("IsTimeSeriesPointValid",
                        col("VR-200-Is-Valid")
                        & col("VR-245-1-Is-Valid")
                        & col("VR-250-Is-Valid")
                        & col("VR-251-Is-Valid")
                        & col("VR-611-Is-Valid")
                        & col("VR-612-Is-Valid")) \
            .drop(col("pd.MarketEvaluationPointType")) \
            .drop(col("pd.QuantityMeasurementUnit_Name")) \
            .drop(col("pd.Product")) \
            .drop(col("pd.SettlementMethod"))
