
# Azure Time Series Insights

Azure Time Series Insights (TSI) offers near real-time data exploration and asset-based insights over historical data.
It is ideal for ad-hoc data exploration and operational analysis.
It also offers visualization engine for time series data, that can be used for aggregations, diagnostic and limited querying.
TSI provides limited functionality for streaming and processing, so it should be considered as a nice-to-have visualization extension.

Architecture top level view:

* TSI can be connected to either input or output event hubs.
* Underlying Data Store of Time Series insight can be used from Databricks or other processing engine
* UI is used for ad-hoc time series analysis, exploration and diagnostics
