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
from pyspark.sql.types import StructType

from geh_stream.validation import Validator

from .streamhandlers.denormalization import denormalize_parsed_data
from .streamhandlers.enricher import Enricher


def parse_enrich_and_validate_time_series_as_points(parsed_data: DataFrame, master_data: DataFrame) -> DataFrame:
    # Denormalize messages: Flatten and explode messages by each contained time series point
    denormalized_data = denormalize_parsed_data(parsed_data)
    print("denormalized_data schema")
    denormalized_data.printSchema()

    enriched_data = Enricher.enrich(denormalized_data, master_data)
    print("Enriched stream schema:")
    enriched_data.printSchema()

    validated_data = Validator.add_validation_status_columns(enriched_data)
    print("Validated stream schema:")
    validated_data.printSchema()

    return validated_data
