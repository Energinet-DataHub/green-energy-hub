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
from pyspark.sql.functions import col
import pyspark.sql.functions as F
from pyspark.sql.utils import AnalysisException


def flatten_df(nested_df: DataFrame) -> DataFrame:
    """
    Flattens a DataFrame and adds nested structure as column name prefixes.

    Example: A "Bar" in a "Foo" struct type would become a new root column named "Foo_Bar".

    See https://stackoverflow.com/questions/38753898/how-to-flatten-a-struct-in-a-spark-dataframe.
    """
    stack = [((), nested_df)]
    columns = []

    while len(stack) > 0:
        parents, df = stack.pop()

        flat_cols = [
            col(".".join(parents + (c[0],))).alias("_".join(parents + (c[0],)))
            for c in df.dtypes
            if c[1][:6] != "struct"
        ]

        nested_cols = [
            c[0]
            for c in df.dtypes
            if c[1][:6] == "struct"
        ]

        columns.extend(flat_cols)

        for nested_col in nested_cols:
            projected_df = df.select(nested_col + ".*")
            stack.append((parents + (nested_col,), projected_df))

    return nested_df.select(columns)


def first(c) -> col:
    """
    In contrast to pyspark.sql.functions.first this function uses column name as alias
    without prefixing it with the aggregation function name.
    """
    if isinstance(c, str):
        return F.first(c).alias(c)

    columnName = c._jc.toString()
    return F.first(c).alias(columnName)


def min(c) -> col:
    """
    In contrast to pyspark.sql.functions.min this function uses column name as alias
    without prefixing it with the aggregation function name.
    """
    if isinstance(c, str):
        return F.min(c).alias(c)

    columnName = c._jc.toString()
    return F.min(c).alias(columnName)


def has_column(df, col):
    try:
        df[col]
        return True
    except AnalysisException:
        return False
