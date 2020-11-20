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
from pyspark.sql.functions import col


class Enricher:

    @staticmethod
    def enrich(parsed_data: DataFrame, master_data: DataFrame):
        return parsed_data.alias("pd") \
            .join(master_data.alias("md"),
                  (col("pd.MarketEvaluationPoint_mRID") == col("md.MarketEvaluationPoint_mRID"))
                  & col("ObservationTime").between(col("md.ValidFrom"), col("md.ValidTo")), how="left") \
            .drop(master_data["MarketEvaluationPoint_mRID"]) \
            .drop(master_data["ValidFrom"]).drop(master_data["ValidTo"])
