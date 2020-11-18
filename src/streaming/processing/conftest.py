"""
By having a conftest.py in this directory, we are able to add all packages
defined in the processing directory in our tests.
"""

import pytest
from pyspark import SparkConf
from pyspark.sql import SparkSession


# Create Spark Conf/Session
@pytest.fixture(scope="session")
def spark():
    spark_conf = SparkConf(loadDefaults=True) \
        .set("spark.sql.session.timeZone", "UTC")
    return SparkSession \
        .builder \
        .config(conf=spark_conf) \
        .getOrCreate()
