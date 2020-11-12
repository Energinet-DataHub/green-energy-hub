from pyspark.sql.functions import col
from processing.codelists import SettlementMethod, MarketEvaluationPointType


# VR.251
#
# The energy quantity for a E20 (exchanging metering point) must be below 1.000.000 kWh,
# else an error message E90 is generated. Again, this is per position.
def validate_vr_251(df):
    productionLimit = 1E6  # 1.000.000 kWh

    return df \
        .withColumn("VR-251-Is-Valid",
                    ~
                    (
                        (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.exchange.value)
                        & col("pd.Quantity").isNotNull()
                        & (col("pd.Quantity") >= productionLimit)
                    ))
