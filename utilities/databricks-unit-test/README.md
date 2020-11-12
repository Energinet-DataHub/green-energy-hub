# PySpark custom action

This action allows you to execute code, written for Spark, Databricks or other similar engines.
It is based on [this](https://docs.github.com/en/free-pro-team@latest/actions/creating-actions/creating-a-docker-container-action) tutorial and running in [jupyter/pyspark-notebook](https://hub.docker.com/r/jupyter/pyspark-notebook) container.

## Usage

In private repository:

```yaml
- name: Unit tests
  uses: ./utilities/databricks-unit-test
```

## Script

```bash
cd ./src/streaming/processing/tests
pytest
```
