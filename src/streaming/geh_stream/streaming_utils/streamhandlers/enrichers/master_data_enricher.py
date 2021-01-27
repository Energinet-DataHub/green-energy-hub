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


def enrich_master_data(parsed_data: DataFrame, master_data: DataFrame):
    # Enrich time series points with master data by joining on market evaluation point mRID and valid period.
    # ValidFrom is inclusive while ValidTo is exclusive.
    joined_data = parsed_data.alias("pd") \
        .join(master_data.alias("md"),
              (col("pd.MarketEvaluationPoint_mRID") == col("md.MarketEvaluationPoint_mRID"))
              & (col("pd.Period_Point_Time") >= col("md.ValidFrom"))
              & (col("pd.Period_Point_Time") < col("md.ValidTo")), how="left")

    # Remove column that are only needed in order to be able to do the join
    return joined_data \
        .drop(parsed_data["MarketEvaluationPoint_mRID"]) \
        .drop(master_data["ValidFrom"]).drop(master_data["ValidTo"])
