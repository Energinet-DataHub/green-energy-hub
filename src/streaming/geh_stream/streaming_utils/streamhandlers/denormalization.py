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
from pyspark.sql.functions import explode, col
from pyspark.sql import DataFrame

from geh_stream.dataframelib import flatten_df


def denormalize_parsed_data(parsed_data: DataFrame) -> DataFrame:
    flattened_parsed_data = flatten_df(parsed_data)
    exploded_data = flattened_parsed_data.select(col("*"), explode(col("Period_Points")).alias("Period_Point")) \
                                         .drop("Period_Points")
    return flatten_df(exploded_data)
