# Data Validation Frameworks for SPARK

## Conditions

* Framework should work as Python (PySpark) library
* Rules type of data validation approach

## Identified options

1. Great Expectations
2. Owl Data Sanitizer
3. PySpark-Check
4. Griffin

## Great Expectations

### Links

* Project page: [Project page](https://greatexpectations.io/)
* Useful blog post: [Blog post](https://www.waitingforcode.com/big-data-problems-solutions/data-validation-frameworks-introduction-greatexpectations/read)

### Key idea

* Define rules or "expectations"

```python
expectation_suite = ExpectationSuite(
    expectation_suite_name='json_test_expectations',
    expectations=[
        {
            'expectation_type': 'expect_column_values_to_not_be_null',
            'kwargs': {
                'column': 'source'
            }
        },
        {
            'expectation_type': 'expect_column_to_exist',
            'kwargs': {
                'column': 'website'
            }
        }
    ]
)
```

* Long list of expectations is available

Few examples:

```python
expect_column_values_to_not_be_null
expect_column_values_to_match_regex
expect_column_values_to_be_unique
expect_column_values_to_match_strftime_format
expect_table_row_count_to_be_between
expect_column_median_to_be_between
```

* Get the result of data frame:

```python
import great_expectations as ge
my_df = ge.read_csv(
    "input.csv",
    expectation_suite=expectation_suite
)
my_df.validate()
```

```json
{
  "results" : [
    {
      "expectation_type": "expect_column_to_exist",
      "success": True,
      "kwargs": {
        "column": "Unnamed: 0"
      }
    },
    ...
    {
      "unexpected_list": 30.3,
      "expectation_type": "expect_column_mean_to_be_between",
      "success": True,
      "kwargs": {
        "column": "Age",
        "max_value": 40,
        "min_value": 10
      }
    },
    {
      "unexpected_list": [],
      "expectation_type": "expect_column_values_to_be_between",
      "success": True,
      "kwargs": {
        "column": "Age",
        "max_value": 80,
        "min_value": 0
      }
    },
    {
      "unexpected_list": [
        "John (?PJ), Smith",
        "Sara Li"
      ],
      "expectation_type": "expect_column_values_to_match_regex",
      "success": True,
      "kwargs": {
        "regex": "[A-Z][a-z]+(?: \\([A-Z][a-z]+\\))?, ",
        "column": "Name",
        "mostly": 0.95
      }
    },
    {
      "unexpected_list": [
        "*"
      ],
      "expectation_type": "expect_column_values_to_be_in_set",
      "success": False,
      "kwargs": {
        "column": "PClass",
        "value_set": [
          "1st",
          "2nd",
          "3rd"
        ]
      }
    }
  ],
  "success", False,
  "statistics": {
      "evaluated_expectations": 10,
      "successful_expectations": 9,
      "unsuccessful_expectations": 1,
      "success_percent": 90.0,
  }
}
```

### Summary

Rules are defined as expectations, not SQL like syntax.

## Owl Data Sanitizer

### Owl Links

* [Project page](https://github.com/ronald-smith-angel/owl-data-sanitizer)
* [Blog post](https://towardsdatascience.com/introducing-a-new-pysparks-library-owl-data-sanitizer-bcc46e1583e6)

### Owl Key idea

* Input table:

```shell
+----------+--------------+--------+---------+------------------+---------+
|GENERAL_ID|          NAME|    CODE|ADDR_DESC|ULTIMATE_PARENT_ID|PARENT_ID|
+----------+--------------+--------+---------+------------------+---------+
|         1|Dummy 1 Entity|12000123|     null|              null|     null|
|         2|          null|    null|     null|                 2|        2|
|         3|          null|12000123|     null|                 3|        3|
|         4|             1|       1|     null|                 4|        4|
|         5|             1|12000123|     null|                 5|        5|
|         6|          null|       3|     null|                 6|        6|
|      null|          null|12000123|     null|                11|        7|
|         7|             2|    null|     null|                 8|        8|
+----------+--------------+--------+---------+------------------+---------+
```

* Rules definition

```json
{
  "source_table": {
    "name": "test.data_test",
    "id_column": "GENERAL_ID",
    "unique_column_group_values_per_table": ["GENERAL_ID", "ULTIMATE_PARENT_ID"],
    "fuzzy_deduplication_distance": 0,
    "output_correctness_table": "test.data_test_correctness",
    "output_completeness_table": "test.data_test_completeness",
    "output_comparison_table": "test.data_test_comparison"
  },
  "correctness_validations": [
    {
      "column": "CODE",
      "rule": "CODE is not null and CODE != '' and CODE != 'null'"
    },
    {
      "column": "NAME",
      "rule": "NAME is not null and NAME != '' and NAME != 'null'"
    },
    {
      "column": "GENERAL_ID",
      "rule": "GENERAL_ID is not null and GENERAL_ID != '' and GENERAL_ID != 'null' and CHAR_LENGTH(GENERAL_ID) < 4"
    }
  ],
  "completeness_validations": [
    {
      "column": "OVER_ALL_COUNT",
      "rule": "OVER_ALL_COUNT <= 7"
    }
  ],
  "parent_children_constraints": [
    {
      "column": "GENERAL_ID",
      "parent": "ULTIMATE_PARENT_ID"
    },
    {
      "column": "GENERAL_ID",
      "parent": "PARENT_ID"
    }
  ],
  "compare_related_tables_list": ["test.diff_df", "test.diff_df_2"]
}
```

* Execute

```python
spark_session = SparkSession.builder.enableHiveSupport().getOrCreate()
with open(PATH_TO_CONFIG_FILE) as f:
        config = Config.parse(f)
CreateHiveValidationDF.validate(spark_session, config)
```

* Results

```shell
+--------------+----------------------------------+-----------------+------------------+-----------------+--------------------------+
|df            |missing_cols_right                |missing_cols_left|missing_vals_right|missing_vals_left|dt                        |
+--------------+----------------------------------+-----------------+------------------+-----------------+--------------------------+
|test.diff_df_2|GENERAL_ID:string,ADDR_DESC:string|GENERAL_ID:int   |                  |                 |2020-04-17 09:39:07.572483|
|test.diff_df  |                                  |                 |6,7               |                 |2020-04-17 09:39:07.572483|
+--------------+----------------------------------+-----------------+------------------+-----------------+--------------------------+
```

### Owl Summary

* SQL type rules definitions

## PySpark-Check

### PySpark-Check Links

[Project page](https://github.com/mikulskibartosz/pyspark-check )

### PySpark-Check Key idea

```python
from pyspark_check.validate_df import ValidateSparkDataFrame
result = ValidateSparkDataFrame(spark_session, spark_data_frame) \
        .is_not_null("column_name") \
        .are_not_null(["column_name_2", "column_name_3"]) \
        .is_min("numeric_column", 10) \
        .is_max("numeric_column", 20) \
        .is_unique("column_name") \
        .are_unique(["column_name_2", "column_name_3"]) \
        .is_between("numeric_column_2", 10, 15) \
        .has_length_between("text_column", 0, 10) \
        .text_matches_regex("text_column", "^[a-z]{3,10}$") \
  .one_of("text_column", ["value_a", "value_b"]) \
        .one_of("numeric_column", [123, 456]) \
        .execute()
result.correct_data # rows that passed the validation
result.erroneous_data # rows rejected during the validation
results.errors a summary of validation errors (three fields: column_name, constraint_name, number_of_errors)
```

### PySpark-Check Summary

The project is at "Alfa" stage. It is probably too early for production.

## Apache Griffin

### Griffin Links

* [Project page](https://griffin.apache.org/)
* [Quick start](https://griffin.apache.org/docs/quickstart.html)
* [Blog post](https://www.programmersought.com/article/3900569253/)

### Griffin Summary

It is a standalone solution for data validation. It is not implemented as Python library.
However, it could be considered as an external command-line tool.
