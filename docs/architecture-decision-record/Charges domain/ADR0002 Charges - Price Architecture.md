# Architecture for Tariff Price Time Series

* Status: approved
* Deciders: @bemwen, @bjarkemeier
* Date: 2021-02-15

Technical Story: Design architectural sketches for processing tariff price time series - ADO-108096

## Context and Problem Statement

Tariff price time series are to be ingested and made available to aggregations in a suitable way. Investigate options and select solution.

Please note that solution for tariff price time series handling should fit well with other price elements (daily tariffs, fees, subscriptions). All of the these must support create, update and delete.

## Considered Options

Ingestion

* Handle price time series ingestion in streaming
* Handle price time series ingestion in market data

Storage

* Market Data DB
* Market Data DB with intermediate ODS in front of aggregations
* Delta Lake

The options above multiply as a number of combinations of which the following have been analyzed

1. Store in Market Data DB
2. Stream to Delta Lake
3. Store in Market Data DB and use for enrichment
4. Stream and use for enrichment(\*)

(\*) Here _enrichment_ has the meaning of using the price time series in the (current) streaming of market evaluation points to enrich the time series points stored in TimeSeries Data Storage.

The options are listed in order of preference.

## Decision Outcome

Chosen option: Option 1 "Store in Market Data DB", because it

* ensures that all price values are stored in the same storage resource (not supported by option 2)
* keeps the current streaming simple and focused (not supported by option 3 and 4)
* keeps architecture simple by not introducing new elements (not supported by any other option)
* does not have _the sync window problem_(*)

(*) Explained below.

The solution architecture

![Solution architecture](0002-price-architecture.png "Solution architecture")

The diagram can be opened and edited using drawio.

### The sync window problem

_The sync window problem_ refers to the problem that streaming in Databricks doesn't continuously lookup data in some storage resource. Instead it batch reads it and updates in at regular intervals. This implies that Market Data DB changes that has been received by the system will not be used in the streaming ingestion until this sync happens again. It is a problem when e.g. a market participant creates a new tariff and immediately following this sends a corresponding tariff price time series. In that case validation will fail.

_The sync window problem_ requires its own special handling.

### Positive Consequences <!-- optional -->

* all price values are stored in the same storage resource
* the current streaming is kept simple and focused
* the architecture does not need to change
* does not expose _the sync window problem_

### Negative Consequences <!-- optional -->

* SQL Server backing the Market Data DB can handle less price data in terms of update frequency (compared to Databricks) and aggregation query demands (compared to Delta Lake)
* history is not supported for free as opposed to a Delta Lake based solution

## Pros and Cons of the Options <!-- optional -->

### [option 1] Store in Market Data DB

All price data including the tariff price time series in question are all directed to Market Data DB. From here they can be directly read from the aggregation jobs.

As a variation it is optional to add an intermediate operation data store (ODS) to reduce the query demand on the Market Data DB from aggregations. It would allow data to be reused from different aggregation jobs and it would allow for incremental updates of the ODS thus also reducing the query demand on the Market Data DB. However, this is not required from a functional perspective and the added work and complexity  should only be added if e.g. analysis of performance or cost suggest that it will pay of.

* Good, because all price values are stored in the same storage resource
* Good, because the current streaming is kept simple and focused
* Good, because the architecture does not need to change
* Good, because the data is stored efficient - i.e. without redundancy (in contrast to e.g. enriching time series points where the same data would be stored multiple times on lots of different points)
* Good, because it does not expose _the sync window problem_
* Bad, because SQL Server backing the Market Data DB can handle less price data in terms of update frequency (compared to Databricks) and aggregation query demands (compared to Delta Lake)
* Bad, because history is not supported for free as opposed to a Delta Lake based solution

The previous concern about the amount of prices was discussed on a meeting Friday (2021-02-12) where it was determined it would not be an issue with the expected sizing of the data.

### [option 2] Stream to Delta Lake

Use Databricks to stream price time series directly into Delta Lake as a separate entity.

* Good, because the current streaming is kept simple and focused
* Good, because the data is stored efficient in terms of required capacity
* Good, because history is supported for free by Delta Lake
* Good, because Databricks can scale to support massive streaming
* Good, because Delta Lake support efficient querying from aggregations
* Bad, because it exposes _the sync window problem_
* Bad, because different price elements are stored in various locations
* Bad, because it requires additional costly Databricks resources
* Bad, because it required additional infrastructure

### [option 3] Store in Market Data DB and use for enrichment

All price data are stored in Market Data DB and used in the current streaming job to enrich metered values.

The following bad reasons are considered enough to discard this option.

* Bad, because it makes the current streaming more complex and less focused and may at the same time have a negative effect on the performance.
* Bad, because it exposes _the sync windows problem_
* Bad, because it requires significantly more storage capacity as lots of identical data will be used for enrichment of multiple metered observations.
* Bad, because it will significantly increase the amount of updates of metered time series points needed

### [option 4] Stream and use for enrichment

All price data are ingested through a streaming job in Databricks and used in the current streaming job to enrich metered values. Storage hasn't been discussed as the option was discarded before.

The same bad reasons that apply to option 3 are considered enough to discard this option.

## Links <!-- optional -->

* [Sizing and retrospective changes of prices](../prices/sizing-and-retrospective-changes-of.prices.md)