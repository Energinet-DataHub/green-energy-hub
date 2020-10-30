# Azure Data Explorer

Azure Data Explorer is quick real-time  exploration engine build originally for complex ad-hoc queries on large volumes of telemetry data and logs.
Query can be used to explore and visualize time series data through an extensive query language suitable for data exploration.
Data Explorer doesn't provide streaming functionality, so cannot be considered a full option.

Architecture top level view:

* Data Explorer can be connected to either input or output event hubs.
  Reference data and other data sources can also be exported on a daily basis.
* SDK can be used to output query results to other parties.
