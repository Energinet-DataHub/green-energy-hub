from pyspark.sql.functions import col
from processing.codelists import SettlementMethod, MarketEvaluationPointType

# VR.250
#
# The energy quantity for a E18 (production metering point) must be below 1.000.000 kWh,
# else an error message E90 is generated. This is per position.
def validate_vr_250(df):
    productionLimit = 1E6 # 1.000.000 kWh

    return df \
        .withColumn("VR-250-Is-Valid",
                     ~
                    (
                        (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.production.value)
                        & col("pd.Quantity").isNotNull()
                        & (col("pd.Quantity") >= productionLimit)
                    )
        )
