from pyspark.sql import DataFrame
from datetime import datetime
from pyspark.sql.functions import col


class TimePeriodFilter:

    @staticmethod
    def filter(df: DataFrame, from_time: datetime, to_time: datetime):
        return df \
            .filter(col('ObservationTime') >= from_time) \
            .filter(col('ObservationTime') <= to_time)
