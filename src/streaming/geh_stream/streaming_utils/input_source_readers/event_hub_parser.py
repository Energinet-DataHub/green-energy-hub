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
from pyspark.sql.functions import col, from_json
from pyspark.sql.types import StructType

json_date_format = "yyyy-MM-dd'T'HH:mm:ss.SSSSSSS'Z'"


class EventHubParser:

    @staticmethod
    def parse(raw_data: DataFrame, message_schema: StructType):
        return raw_data \
            .select(from_json(col("body").cast("string"),
                              message_schema,
                              options={"dateFormat": json_date_format}).alias("message"),
                    col("enqueuedTime").alias("EventHubEnqueueTime")) \
            .select(col("message.*"), col("EventHubEnqueueTime"))
