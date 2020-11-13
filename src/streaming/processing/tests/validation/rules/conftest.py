import pytest
import pandas as pd
import time
from pyspark import SparkConf
from pyspark.sql import DataFrame, SparkSession
from pyspark.sql.types import DoubleType, StringType, StructField, StructType, TimestampType
from pyspark.sql.functions import col, lit
from processing.codelists import MarketEvaluationPointType, Quality
from processing.streaming_utils import Enricher
from processing.schemas import SchemaNames, SchemaFactory

# Create timestamps used in DataFrames
time_now = time.time()
time_future = time.time() + 1000
time_past = time.time() - 1000
timestamp_now = pd.Timestamp(time_now, unit='s')
timestamp_future = pd.Timestamp(time_future, unit='s')
timestamp_past = pd.Timestamp(time_past, unit='s')


# Create schema of parsed data (timeseries data) and master data
@pytest.fixture(scope="session")
def parsed_schema():
    return SchemaFactory.get_instance(SchemaNames.Parsed)


@pytest.fixture(scope="session")
def master_schema():
    return SchemaFactory.get_instance(SchemaNames.Master)


# Create parsed data and master data Dataframes
@pytest.fixture(scope="session")
def master_data_factory(spark, master_schema):
    def factory(market_evaluation_point_type, settlement_method):
        pandas_df = pd.DataFrame({
            'MarketEvaluationPoint_mRID': ["1"],
            "ValidFrom": [timestamp_past],
            "ValidTo": [timestamp_future],
            "MeterReadingPeriodicity": ["a"],
            "MeterReadingPeriodicity2": ["c"],
            "MeteringMethod": ["b"],
            "MeteringGridArea_Domain_mRID": ["d"],
            "ConnectionState": ["e"],
            "EnergySupplier_MarketParticipant_mRID": ["f"],
            "BalanceResponsibleParty_MarketParticipant_mRID": ["g"],
            "InMeteringGridArea_Domain_mRID": ["h"],
            "OutMeteringGridArea_Domain_mRID": ["i"],
            "Parent_Domain": ["j"],
            "SupplierAssociationId": ["k"],
            "ServiceCategoryKind": ["l"],
            "MarketEvaluationPointType": [market_evaluation_point_type],
            "SettlementMethod": [settlement_method],
            "UnitName": ["o"],
            "Product": ["p"]})
        return spark.createDataFrame(pandas_df, schema=master_schema)
    return factory


@pytest.fixture(scope="session")
def parsed_data_factory(spark, parsed_schema):
    def factory(quantity):
        pandas_df = pd.DataFrame({
            "MarketEvaluationPoint_mRID": ["1"],
            "ObservationTime": [timestamp_now],
            "Quantity": [quantity],
            "CorrelationId": ["a"],
            "MessageReference": ["b"],
            "HeaderEnergyDocument_mRID": ["c"],
            "HeaderEnergyDocumentCreation": [timestamp_now],
            "HeaderEnergyDocumentSenderIdentification": ["d"],
            "EnergyBusinessProcess": ["e"],
            "EnergyBusinessProcessRole": ["f"],
            "TimeSeriesmRID": ["g"],
            "MktActivityRecord_Status": ["h"],
            "Product": ["i"],
            "UnitName": ["j"],
            "MarketEvaluationPointType": [MarketEvaluationPointType.consumption.value],
            "Quality": [Quality.as_read.value],
            "EventHubEnqueueTime": [timestamp_now]})
        return spark.createDataFrame(pandas_df, schema=parsed_schema)
    return factory


@pytest.fixture(scope="session")
def enriched_data_factory(parsed_data_factory, master_data_factory):
    def creator(quantity=1.0,
                market_evaluation_point_type="m",
                settlement_method="n"):
        parsed_data = parsed_data_factory(quantity)
        master_data = master_data_factory(market_evaluation_point_type, settlement_method)
        return Enricher.enrich(parsed_data, master_data)

    return creator
