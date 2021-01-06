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
from .enrichers.master_data_enricher import enrich_master_data
from .enrichers.recipient_enricher import enrich_recipients


class Enricher:

    @staticmethod
    def enrich(parsed_data: DataFrame, master_data: DataFrame):
        master_data_enriched = enrich_master_data(parsed_data, master_data)
        recipient_enriched = enrich_recipients(master_data_enriched)

        return recipient_enriched
