from pyspark.sql.functions import col
from processing.codelists import Quality, MarketEvaluationPointType


# VR.245-1:
# The energy quantity of a consumption/metered data reading must not be a negative number
def validate_vr_245_1(df):
    return df \
        .withColumn("VR-245-1-Is-Valid",
                    ~
                    (
                        (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value)
                        & col("Quantity").isNotNull()
                        & (col("Quantity") < 0)
                    ))
