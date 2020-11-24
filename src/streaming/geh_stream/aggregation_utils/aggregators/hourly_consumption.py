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
from pyspark.sql.functions import col, window
from geh_stream.codelists import MarketEvaluationPointType, SettlementMethod


class HourlyConsumptionAggregator:

    @staticmethod
    def aggregate(df: DataFrame):
        return df \
            .filter(col("MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value) \
            .filter(col("SettlementMethod") == SettlementMethod.non_profiled.value) \
            .groupBy("MeteringGridArea_Domain_mRID",
                     "BalanceResponsibleParty_MarketParticipant_mRID",
                     "EnergySupplier_MarketParticipant_mRID",
                     window(col("ObservationTime"), "1 hour")) \
            .sum("Quantity") \
            .withColumnRenamed("sum(Quantity)", "sum_quantity") \
            .withColumnRenamed("window", "time_window") \
            .orderBy("MeteringGridArea_Domain_mRID",
                     "BalanceResponsibleParty_MarketParticipant_mRID",
                     "EnergySupplier_MarketParticipant_mRID",
                     "time_window")
