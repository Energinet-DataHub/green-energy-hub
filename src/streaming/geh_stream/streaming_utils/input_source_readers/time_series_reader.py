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
from pyspark.sql import SparkSession, DataFrame
from pyspark.sql.types import StructType

from geh_stream.schemas import SchemaFactory, SchemaNames
from .event_hub_parser import EventHubParser


def read_time_series_streaming_data(spark: SparkSession, input_eh_conf: dict) -> DataFrame:
    raw_data = spark \
        .readStream \
        .format("eventhubs") \
        .options(**input_eh_conf) \
        .option("inferSchema", True) \
        .load()

    print("Input stream schema:")
    raw_data.printSchema()

    message_schema: StructType = SchemaFactory.get_instance(SchemaNames.MessageBody)
    parsed_data = EventHubParser.parse(raw_data, message_schema) \
        .withColumnRenamed("mRID", "TimeSeries_mRID")

    print("Parsed stream schema:")
    parsed_data.printSchema()

    return parsed_data
