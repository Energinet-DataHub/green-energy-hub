# Azure Synapse

Azure Synapse is basically next generation of Azure SQL Data Warehouse and unites modern data warehousing under a single technology: Spark for Big Data processing and aggregations + multi-node SQL Server for ad-hoc queries and reporting.

Architecture top level view:

* Single Synapse workspace for relational and Data Lake processing.
* On-demand auto-scalable Spark cluster for batch jobs, streaming and experimentation.
* Data is stored in parquet format on Azure Data Lake.
  Delta Lake provider is preconfigured.
* Applications are notebooks running automatically cell-by-cell or application packages deployed as Spark jobs (.scala/.py/C# files)
* Streaming and batch processing are preconfigured on Synapse level as jobs.
* Delta Lake is preinstalled, so Delta Lake specific features from above are available as well.
* External SQL connection is used to copy reference data into memory of spark job.
  Streaming job is restarted daily to update reference cache.

SQL Data Warehouse functionality offered by Azure Synapse can be used as [additional analytical exploration engine](azure-data-warehouse.md).

Architecture top level view:

* Data to Azure Synapse SQL should be ingested either by Spark output or Data Factory from Data Lake.
  Reference data and other data sources can also be imported on a daily basis.
* OLEDB/ODBC can be used to output query results to other parties.
