# Use of Change Data Capture (SQL Server)

* Status: proposed
* Deciders: @xmspe @prtandrup @perthenriksen @MartinFHansen
* Date: 2021-01-29

## Context and Problem Statement

We want to be able to make Audits and see old revisions of data

## Considered Options

* Change Data Capture [CDC](https://docs.microsoft.com/en-us/sql/relational-databases/track-changes/about-change-data-capture-sql-server?view=sql-server-ver15)
* Revision of documents in the same tables
* History / Archive table created of each table

### Positive Consequences of using CDC

* We get a tried and tested solution
* We save development time by picking an off the shelf solution

### Negative Consequences of using CDC

* Since we do not know our query requirements for getting old revisions of data and we do not have any performance testing, CDC might not be fast enough.
* If we have any special requirements it will be harder/impossible to customize CDC

## Decision Outcome

The decision is not final, but currently we go with Change Data Capture (SQL Server) because

* As of now we do not have the full business requirements, this makes it hard to not either under or over engineer a self made solution
* By not having having revisions of rows we help query speed