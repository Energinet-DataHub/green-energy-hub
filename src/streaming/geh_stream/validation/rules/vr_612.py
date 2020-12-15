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


# VR.612
#
# The energy quantity for a E17 (consumption metering point) must be
# below 1.000.000 kWh for flex settled metering point, else E90 is generated. This is per position.
#
# It is not necessary to check that the resolution is hourly because it is given when settlement method is flex.
def validate_vr_612(df):
    consumptionLimit = 1E6  # 1.000.000 kWh

    return df \
        .withColumn("VR-612-Is-Valid",
                    ~
                    (
                        col("pd.Period_Point_Quantity").isNotNull()
                        & (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value)
                        & (col("md.SettlementMethod") == SettlementMethod.flex_settled.value)
                        & (col("pd.Period_Point_Quantity") >= consumptionLimit)
                    ))
