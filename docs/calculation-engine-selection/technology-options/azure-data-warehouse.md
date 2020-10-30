# Azure SQL Data Warehouse

Azure Data Warehouse is an analytical exploration engine build for hot relational big data processing ready for terabytes of data.
Azure Data Warehouse can be used to explore aggregated data through SQL query language, but by itself it's not enough for streaming or big data schemaless processing.

Architecture top level view:

* Data to Azure SQL Data Warehouse should be ingested either by Spark output or Data Factory from Data Lake.
  Reference data and other data sources can also be imported on a daily basis.
* OLEDB/ODBC can be used to output query results to other parties.
* Azure Synapse now comprises SQL Data Warehouse
