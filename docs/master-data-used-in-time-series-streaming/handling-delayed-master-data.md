# Master data used in time series streaming

## Context

Time series information in the GreenEnergyHub is handled in a streaming process where the various time series’ are validated, enriched with master data, stored to the delta lake and forwarded to any necessary recipient.

Since the amount of data flowing through this part of the system is enormous, the enrichment of master data is not done by looking up the data directly in a database or service. Instead, master data is periodically read into the memory of the Databricks job handling the enrichment. The master data from memory is then used when enriching and validating the data before it is saved and forwarded.

Because of this, and because its an asynchronous solution in general, master data might be slightly out of sync when it is enriched, validated, stored, and forwarded. The solution is eventual consistent, and the processing of the time series might occur while it does not have the latest changes.

To solve this, a mechanism - for fixing time series data that was previously validated successfully and stored using master data with an out of date state - must be invented. The goal of the mechanism is to bring the data back in a consistent state.

## Architectural area

In this document, we will be addressing the Time Series Data Storage, Market Data Event Queue, the Updated Master Data notebook, the Validation Master Data Enrichment notebook, the TimeSeries ODS and distribution list ODS seen in the general architecture.

![Technical Stack Diagram](../images/TechStack.png)

## Basic concept

Master data needed in the enrichment job is fetched through the Operational Data Storage (ODS) for master data or the distribution list ODS. This will continue to be the case. No changes will be needed for either of these operational data storages nor the enrichment job itself.

In addition to the normal processing of the time series, the Updated Master Data mechanism is extended to handle both retrospective master data changes and any issues that might have been caused by the delay in master data used in Databricks. It will be triggered on certain master data changes. These changes will be visible in the Market Data Event Queue where the events will be raised once a change has occurred. The mechanism will then react to these changes and re-enrich and re-send the time series information. Essential to the solution is that the events will be delayed so that we are sure the master data changes have actually reached the memory of the Databricks jobs.

## Market Data Event Queue

All events triggered by processes in the market that update master data will make their way through the Market Data Event Queue.

On the queue various events will be raised. Not all of them are relevant to the handling of time series information, but for those that are, a job can be triggered for further processing. The events that are relevant will be the ones that can update the data used for enrichment and validation or events that require retrospective changes (such as move-in processes operating in the past).

To be able to handle both retrospective changes and changes caused by slightly outdated master data, it is important that the handling of the events are delayed by at least as long a time as it takes for the data to reach the memory in Databricks.

For each event the following information is necessary:

* Business reason code or type of event
* Effective DateTime
* Market Evaluation Point

## Updated Master Data notebook

The Updated Master Data notebook is responsible for handling retrospective changes and cleaning up any time series data stored in the data storage which might have been saved with out of date master data.

The notebook will be triggered by the delayed events originating in the Market Data Event Queue.

The effective DateTime of the event is used to lookup time series information in the data storage for the market evaluation point in question. All data from the effective DateTime and forward is considered. No matter the type of the event the data will be re-enriched with (possible) updated master data and resend to the recipients determined by the distribution list.

In addition to the re-enrichment and re-sending, some types of events will also require additional operations before these steps are handled. Some events require a soft delete of the time series data before it is handled. This is for example the case where a market evaluation point changes time series resolution from hourly to 15 minutes. In these cases, the data is soft deleted before re-enrichment and re-sending takes place to ensure that these last two vital steps occur on the correct data.

## Child market evaluation points

Some type of events can only occur on parent market evaluation points. A move-in process may only occur on the parent and not on any of the child market evaluation points. This means that master data changes of a parent can potentially also trigger changes to master data for child market evaluation points, e.g. supplier information is stored on the time series data for child metering points and thus must be handled if the supplier changes on the parent. Some events handled for a parent should therefore also trigger re-enrichment for child market evaluation points. The notebook will need to be aware of this as there will not be separate events for the children themselves.

Events on child market evaluation points can't in the same way influence the parent, so they can be handled as usual.

## Quantity missing and message rejection

In some cases, changes to master data will mean that data has been send out to the wrong recipients previously. In these cases, it has been discussed whether the solution should send a new message resetting these data using the quantity missing status. It has however been determined that sending out quantity missing is undesirable as it might cause problems for the recipients.

Likewise, it has been discussed whether a rejection message should be sent out to the sender of the original message if data was later found to be incorrect. It has been determined that this is not worth the effort and complexity needed to do so.
