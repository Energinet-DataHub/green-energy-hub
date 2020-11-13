from decimal import Decimal
import pandas as pd
from processing.aggregation_utils.aggregators import HourlyConsumptionSupplierAggregator
from processing.codelists import MarketEvaluationPointType, SettlementMethod
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import StructType, StringType, DecimalType
import pytest

e_17 = MarketEvaluationPointType.consumption.value
e_18 = MarketEvaluationPointType.production.value
e_01 = SettlementMethod.profiled.value
e_02 = SettlementMethod.non_profiled.value


# Time series schema
@pytest.fixture(scope="module")
def time_series_schema():
    return StructType() \
        .add("MarketEvaluationPointType", StringType(), False) \
        .add("SettlementMethod", StringType()) \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType()) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType()) \
        .add("Quantity", DecimalType())


@pytest.fixture(scope="module")
def expected_schema():
    """
    Expected aggregation schema
    NOTE: Spark seems to add 10 to the precision of the decimal type on summations.
    Thus, the expected schema should be precision of 20, 10 more than the default of 10.
    If this is an issue we can always cast back to the original decimal precision in the aggregate
    function.
    https://stackoverflow.com/questions/57203383/spark-sum-and-decimaltype-precision
    """
    return StructType() \
        .add("MeteringGridArea_Domain_mRID", StringType(), False) \
        .add("EnergySupplier_MarketParticipant_mRID", StringType()) \
        .add("BalanceResponsibleParty_MarketParticipant_mRID", StringType()) \
        .add("sum_quantity", DecimalType(20))


@pytest.fixture(scope="module")
def time_series_data_frame(spark, time_series_schema):
    """
    Sample Time Series DataFrame
    Here we create 2 quantity values for each "grid domain area"-"supplier"-"balance responsible" grouping (16 total)
    We create 2 of each grouping to test whether the summation in the aggregation occurs.
    This also gives us an opportunity to have a non "E17" MarketEvaluationPointType in one or both of the
    time series rows (rows with non "E17" should get filtered out in the aggregation).
    We refer to the grouping as D#-S#-R# groupings where D=Domain, S=Supplier R=Responsible
    """
    # Create empty pandas df
    pandas_df = pd.DataFrame({
        'MarketEvaluationPointType': [],
        'SettlementMethod': [],
        "MeteringGridArea_Domain_mRID": [],
        "EnergySupplier_MarketParticipant_mRID": [],
        "BalanceResponsibleParty_MarketParticipant_mRID": [],
        "Quantity": []
    })
    # Add sample data row by row to the data frame
    # D1-S1-R1 (both rows will be filtered away because MeteringPointType="E18")
    pandas_df = add_row_of_data(pandas_df, e_18, e_02, "D1", "S1", "R1", Decimal(1))
    pandas_df = add_row_of_data(pandas_df, e_18, e_02, "D1", "S1", "R1", Decimal(9))
    # D1-S1-R2 (the second row will be filtered away because MeteringPointType="E18")
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D1", "S1", "R2", Decimal(2))
    pandas_df = add_row_of_data(pandas_df, e_18, e_02, "D1", "S1", "R2", Decimal(10))
    # D1-S2-R1 (the second row will be filtered away because SettlementMethod="E01")
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D1", "S2", "R1", Decimal(3))
    pandas_df = add_row_of_data(pandas_df, e_17, e_01, "D1", "S2", "R1", Decimal(11))
    # D1-S2-R2
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D1", "S2", "R2", Decimal(4))
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D1", "S2", "R2", Decimal(12))
    # D2-S1-R1
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S1", "R1", Decimal(5))
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S1", "R1", Decimal(13))
    # D2-S1-R2
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S1", "R2", Decimal(6))
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S1", "R2", Decimal(14))
    # D2-S2-R1
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S2", "R1", Decimal(7))
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S2", "R1", Decimal(15))
    # D2-S2-R2
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S2", "R2", Decimal(8))
    pandas_df = add_row_of_data(pandas_df, e_17, e_02, "D2", "S2", "R2", Decimal(16))
    return spark.createDataFrame(pandas_df, schema=time_series_schema)


def add_row_of_data(pandas_df: pd.DataFrame, point_type, settlement_method, domain, supplier, responsible, quantity):
    """
    Helper method to create a new row in the dataframe to improve readability and maintainability
    """
    new_row = {
        "MarketEvaluationPointType": point_type,
        "SettlementMethod": settlement_method,
        "MeteringGridArea_Domain_mRID": domain,
        "EnergySupplier_MarketParticipant_mRID": supplier,
        "BalanceResponsibleParty_MarketParticipant_mRID": responsible,
        "Quantity": quantity
    }
    return pandas_df.append(new_row, ignore_index=True)


@pytest.fixture(scope="module")
def aggregated_data_frame(time_series_data_frame):
    """Perform aggregation"""
    return HourlyConsumptionSupplierAggregator.aggregate(time_series_data_frame)


def test_hourly_consumption_supplier_aggregator_returns_correct_row_count(aggregated_data_frame):
    """ Check aggregation row count"""
    assert aggregated_data_frame.count() == 7


def test_hourly_consumption_supplier_aggregator_returns_correct_schema(aggregated_data_frame, expected_schema):
    """Check aggregation schema"""
    assert aggregated_data_frame.schema == expected_schema


def test_hourly_consumption_supplier_aggregator_returns_correct_aggregations(aggregated_data_frame):
    """Check accuracy of aggregation"""
    # Check all 8 rows of the resultant table using helper function
    check_aggregation_row(aggregated_data_frame, 0, "D1", "S1", "R2", Decimal(2))
    check_aggregation_row(aggregated_data_frame, 1, "D1", "S2", "R1", Decimal(3))
    check_aggregation_row(aggregated_data_frame, 2, "D1", "S2", "R2", Decimal(16))
    check_aggregation_row(aggregated_data_frame, 3, "D2", "S1", "R1", Decimal(18))
    check_aggregation_row(aggregated_data_frame, 4, "D2", "S1", "R2", Decimal(20))
    check_aggregation_row(aggregated_data_frame, 5, "D2", "S2", "R1", Decimal(22))
    check_aggregation_row(aggregated_data_frame, 6, "D2", "S2", "R2", Decimal(24))


def check_aggregation_row(df: DataFrame, row: int, grid: str, supplier: str, responsible: str, sum: Decimal):
    """Helper function that checks column values for the given row"""
    pandas_df = df.toPandas()
    assert pandas_df["MeteringGridArea_Domain_mRID"][row] == grid
    assert pandas_df["EnergySupplier_MarketParticipant_mRID"][row] == supplier
    assert pandas_df["BalanceResponsibleParty_MarketParticipant_mRID"][row] == responsible
    assert pandas_df["sum_quantity"][row] == sum
