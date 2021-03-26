# Event data in Green Energy Hub integration events

- Status: accepted
- Deciders: @Energinet-DataHub/mightyducks @MartinFHansen
- Date: 2021-03-19

## Context and Problem Statement

Green Energy Hub is platform composed of several domains that need to exchange data through integration events to realize the intended business value. The inter-domain communication between the domains must be flexible, extensible, and resilient to break downs in other parts of the platform.

## Considered Options

- Option 1: Events informing that an action has occured (e.g. a market evaluation point has been created) without any additional event data.
- Option 2: Events informing that an action has occured (e.g. a market evaluation point has been created) with additional event data (e.g. ID and location of the new market evaluation point).

## Decision Outcome

Option 2 has been chosen because this option is the best fit to support a loosely coupled architecture, where the domains are flexible, extensible and resilient.

### Positive Consequences

- A domain always publishes "the full picture" of that happened.
- After consuming an event a domain won't have to inquire the publishing domain for additional information regarding the action that occured in the platform.
- A domain is independent of availability of the applications running in other domains.

### Negative Consequences

- An event consuming domain may not need all data available in an event.
- When a new property is added to an event, the Protobuf contract describing the integration event must be extended, and thereby the version must be increased.

## Pros and Cons of the Options

### [option 1]

- Good, because once an integration event is created it won't change over time.
- Bad, because it requires a consuming domain to inquire for the data its needs from the publishing domain in a separate call, e.g. using an REST API.
- Bad, because it makes a domain depended on the availability of another domain.
- Bad, because if an event consuming domain needs additional data from the publishing domain, the API to fetch the data must be extended.

### [option 2]

- Good, because it facilitates the inter-domain communication to be purely event-driven. NB.: The exeption to this rule being communication from the API gateway domain, which must return synchronous responses to the market actors.
- Good, because inter-domain communication in Green Energy Hub is done using one communication paradigm.
- Good, because domains are not depended on the availability of applications in other domains.
- Bad(ish), because the Protobuf contracts that describe the integration events will be extended over time with new properties being sent (results in a new version of a contract).
    - If used correctly Protobuf contracts are backward and forward compatible, see links.

## Links

- [Backward and Forward Compatibility, Protobuf Versionin](https://www.beautifulcode.co/blog/88-backward-and-forward-compatibility-protobuf-versioning-serialization)
