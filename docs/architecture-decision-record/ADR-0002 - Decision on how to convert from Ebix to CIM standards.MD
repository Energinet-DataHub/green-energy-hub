# ADR0002 Decision on how to convert from Ebix to CIM standards

* Status: [Accepted] - But needs to be re-written following decisions regarding use of Ebix vs. CIM
* Deciders: Martin F. Hansen, René Thyge Nielsen, Daniel Bjerring Jørgensen, Søren Søndergaard
* Date: 2021-03-18 (PI4 Planning event)

Technical Story: <https://github.com/Energinet-DataHub/geh/issues/6>

## Context and Problem Statement

The Danish electricty market are communicating with the Datahub using a Ebix formated message content for both incoming and outgoing messages. The decision for the Datahub 3.0 is that all internal communication are done in a CIM (ENTSO-e format) formatting. To be able to implement CIM conversion it has been decided that the system team will provide a 'CIM dictionary' for following RSM-messages (EbiX) 001, 004, 033, a conversion-engine and a guide for how all teams are able to implement the conversion engine and create CIM aligned data within their repective domains.

## Decision Outcome

Chosen option: Each domain have the freedom to do their own conversion to CIM or internal format by using a centralized aligned 'dictionary' and tool created for this. This option that supports all decision drivers, giving an aligned naming according to ENTSO-e standard, ensure team responsibility but on an aligned format for internal implementation within domains.

## Decision Drivers

* Driver 1: Responsibility for the dictionary of EbiX-CIM needs to be centralized as changes to formats comes from the business side, and one entry point is necessary to maintain an aligned format across the system.
* Driver 2: We do not wish to create a bottleneck for the flows by creating one common conversion for all domains/business processes.
* Driver 3: We are able to create one converter, that all teams can implement within the host layer of each domain - containing all relevant data for the domain.
* Driver 4: We would like for the domains to be able to create their internal DTO's based on needed data - but still aligned with CIM naming.

## Considered Options

* 1. It was considered to deliver one conversion tool within message entry point of the system (API management)
* 2. It was considered that each domain had the freedom to do their own conversion to CIM or internal format by using a centralized aligned 'dictionary' and tool created for this.
* 3. It was considered that the system team would implement all conversion tools in domain across the platform

### Positive Consequences

* No bottleneck created in API management
* One entry point of changes to naming and formats from the business
* One tool, that can be used in all domains
* Possible for each domain to create internal DTO's
