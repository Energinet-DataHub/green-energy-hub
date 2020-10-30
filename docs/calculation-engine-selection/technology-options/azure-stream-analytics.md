
# Azure Stream Analytics for data enrichment/validation

Azure Stream Analytics is a scalable real-time data processing engine based on SQL-like query language. It can provide the easiest approach to initial data aggregation and real-time processing. It doesn't contain any batch processing functionality, so that cannot be considered as a full-featured option.

Architecture top level view:

* This option doesn't have any batch processing functionality for analytical workloads that is why being mainly limited to data enrichment and validation workloads.
* While SQL is very expressive and allows User Defined Functions (UDF), Stream Analytics still lacks some extensibility like Spark-based technologies.
* Stream Analytics job listens to Event Hub and processes data on arrival using prebuilt late arrivals handling.
* Reference data in SQL Database is fetched on a schedule (through T-SQL delta queries) and then used in streaming.
* Invalid data is forwarded to another target (Event Hub or queue) for further processing.
* Data stored in parquet format on Azure Data Lake connected to the clusters.
* Applications are SQL queries with metadata defined connections. Visual Studio Code extension is available.
* Data duplicates and updates are stored "as-is" in append-only format and then filtered during the first stage of processing and aggregation queries.
