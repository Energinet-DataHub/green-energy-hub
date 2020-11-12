from pyspark.sql.functions import col
from processing.codelists import SettlementMethod, MarketEvaluationPointType


# VR.611
#
# The energy quantity for a E17 (consumption metering point) must be below 100.000  kwh for hour settled,
# else generate an error message E90 is generated (according to VR 611). This is per position.
# The sender can choose not to assign a value to energy quantity, this is accepted.
#
# It is not necessary to check that the resolution is hourly because it is given when settlement method is non-profiled.
def validate_vr_611(df):
    consumptionLimit = 1E5  # 100.000 kWh

    return df \
        .withColumn("VR-611-Is-Valid",
                    ~  # Negate the below expression to make it an is-valid instead of is-invalid
                    (
                        # Expression for the exact situation where the violation is determined to have occurred
                        col("pd.Quantity").isNotNull()
                        & (col("md.MarketEvaluationPointType") == MarketEvaluationPointType.consumption.value)
                        & (col("md.SettlementMethod") == SettlementMethod.non_profiled.value)
                        & (col("pd.Quantity") >= consumptionLimit)
                    ))
