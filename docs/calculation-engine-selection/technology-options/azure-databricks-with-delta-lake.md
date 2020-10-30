# Azure Databricks with Delta Lake

Delta Lake is an additional feature of Databricks solution that brings ACID transactions, read-time optimizations through primary indexes, time travel, schema enforcement and evolution functionalities.
Databricks adds additional performance optimizations for Delta Lake scenarios.

Architecture top level view:

* **Same as for Azure Databricks** plus
* Transaction and time-travel features are used for storing data in "snapshots": current state is available by default, but additional query parameters can be used to restore state to a specific point in time.
  It also means the existing data can be altered by incoming updates, thus updates are not necessarily applied only in append format.
* Primary index is set to conform to the most common data partitioning and ordering through aggregation queries.
