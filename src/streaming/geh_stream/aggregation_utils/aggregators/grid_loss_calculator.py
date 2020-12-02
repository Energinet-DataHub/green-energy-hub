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


class GridLossCalculator:

    @staticmethod
    def calculate(agg_net_exchange: DataFrame, agg_hourly_consumption: DataFrame, agg_flex_consumption: DataFrame, agg_production: DataFrame):
        grid_area = "MeteringGridArea_Domain_mRID"

        agg_net_exchange_result = agg_net_exchange.selectExpr(grid_area, "result as net_exchange_result")
        agg_hourly_consumption_result = agg_hourly_consumption.selectExpr(grid_area, "sum_quantity as hourly_result")
        agg_flex_consumption_result = agg_flex_consumption.selectExpr(grid_area, "sum_quantity as flex_result")
        agg_production_result = agg_production.selectExpr(grid_area, "sum_quantity as prod_result")

        result = agg_net_exchange_result.join(agg_production_result, [grid_area]).join(agg_hourly_consumption_result.join(agg_flex_consumption_result, grid_area), grid_area)
        result = result.withColumn('grid_loss', result.net_exchange_result + result.prod_result - (result.hourly_result + result.flex_result))

        return result.select(grid_area, "grid_loss")
