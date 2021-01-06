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
import os
import pandas as pd
import pytest
from pyspark.sql import DataFrame

from geh_stream.schemas import SchemaNames, SchemaFactory
import geh_stream.streaming_utils.input_source_readers.event_hub_parser as eventhub

testdata_dir = os.path.dirname(os.path.realpath(__file__)) + "/testdata/"


def read_testdata_file(file_name):
    with open(testdata_dir + file_name, "r") as f:
        return f.read()


@pytest.fixture(scope="session")
def parsed_data_from_json_file_factory(spark):
    """
    Create parsed data from file in ./templates folder.
    """
    parsed_data_schema = SchemaFactory.get_instance(SchemaNames.Parsed)

    def factory(file_name) -> DataFrame:
        json_str = read_testdata_file(file_name)
        json_rdd = spark.sparkContext.parallelize([json_str])
        parsed_data = spark.read.json(json_rdd,
                                      schema=parsed_data_schema,
                                      dateFormat=eventhub.json_date_format)
        return parsed_data

    return factory


@pytest.fixture(scope="session")
def master_data(spark):
    master_data_schema = SchemaFactory.get_instance(SchemaNames.Master)
    # dtype=str to prevent pandas from inferring types, which e.g. causes some strings to be parsed as floats and displayed in scientific notation
    master_data_pd = pd.read_csv(testdata_dir + "master-data.csv",
                                 sep=";",
                                 parse_dates=["ValidFrom", "ValidTo"],
                                 dtype=str)
    # Remove the meta column. The column is added in test csv file to let developers explain the data in the rows.
    master_data_pd.drop(columns=["Meta"], inplace=True)
    master_data = spark.createDataFrame(master_data_pd, schema=master_data_schema)
    return master_data
