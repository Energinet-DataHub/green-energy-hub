from pyspark.sql.functions import col
from processing.codelists import SettlementMethod, MarketEvaluationPointType


# VR.612
#
# The energy quantity for a E17 (consumption metering point) must be
# below 1.000.000 kWh for flex settled metering point, else E90 is generated. This is per position.
#
# It is not necessary to check that the resolution is hourly because it is given when settlement method is flex.
def validate_vr_612(df):
    consumptionLimit = 1E6  # 1.000.000 kWh

    return df \
        .withColumn("VR-612-Is-Valid",
                    ~
                    (
                        col("pd.Quantity").isNotNull()
                        & (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value)
                        & (col("md.SettlementMethod") == SettlementMethod.flex_settled.value)
                        & (col("pd.Quantity") >= consumptionLimit)
                    ))
