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
from pyspark.sql.functions import coalesce, lit, col

from geh_stream.schemas import SchemaFactory, SchemaNames

csv_read_config = {
    "inferSchema": "True",
    "delimiter": ";",
    "header": "True",
    "nullValues": "NULL"
}

master_data_schema = SchemaFactory.get_instance(SchemaNames.Master)


def read_master_data_from_csv(spark: SparkSession, master_data_storage_path: str) -> DataFrame:
    master_data = spark \
        .read \
        .format("csv") \
        .schema(master_data_schema) \
        .options(**csv_read_config) \
        .load(master_data_storage_path)

    master_data = master_data \
        .withColumn("ValidTo",
                    coalesce(col("ValidTo"), lit("9999-12-31").cast("timestamp")))

    master_data.printSchema()
    master_data.show()
    return master_data
