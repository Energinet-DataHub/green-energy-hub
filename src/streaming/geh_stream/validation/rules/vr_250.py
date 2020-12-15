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
from pyspark.sql.functions import col
from geh_stream.codelists import SettlementMethod, MarketEvaluationPointType


# VR.250
#
# The energy quantity for a E18 (production metering point) must be below 1.000.000 kWh,
# else an error message E90 is generated. This is per position.
def validate_vr_250(df):
    productionLimit = 1E6  # 1.000.000 kWh

    return df \
        .withColumn("VR-250-Is-Valid",
                    ~
                    (
                        (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.production.value)
                        & col("pd.Period_Point_Quantity").isNotNull()
                        & (col("pd.Period_Point_Quantity") >= productionLimit)
                    ))
