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
from datetime import datetime
from pyspark.sql.functions import col


class TimePeriodFilter:

    @staticmethod
    def filter(df: DataFrame, from_time: datetime, to_time: datetime):
        return df \
            .filter(col('ObservationTime') >= from_time) \
            .filter(col('ObservationTime') <= to_time)
