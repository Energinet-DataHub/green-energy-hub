# How to test integration points within domains prior to deploying to DEV (U) environment

* Status: [Proposed]
* Deciders: []
* Date: [YYYY-MM-DD when the decision was last updated] <!-- optional -->

Technical Story: [description | ticket/issue URL] <!-- optional -->

***DELETE THIS IN ADR: Remember to update the ADR index (found in [README.md](README.md)) with the new ARD. Depending on the topic of the new ARD either add it to the existing list of ADR topics OR add a new entry in the list of ADR topics.***

## Context and Problem Statement

How can we leverage integration testing, so that it

* Increases our software confidence
* Supports an efficient development process
* Enables a fast and reliable feedback loop
* Guards the DEV environment from becoming unstable and thereby increases DEV availability
* Enables us to ensure connectivity and data correctness

To be able to ensure that our integrations componenets are set up correctly and functional with expected behavior before deploying to DEV (U). We need to use a framework that allows us to create local instances for testing prior to deployment to DEV environment.

## Decision Drivers

* We need an integration test solution that fits a microservice architecture as well as the practices of DevOps
* The solution must solve the problem with cloud components that cannot be emulated, specifically for local testing efforts
* We emphasize failing fast, having quick and reliable feedback loops and DEV environment stability and availability
* We want a solution that does not require a full domain setup in an environment
* The solution must be easy to set up and tear down as part of PR gate or deployment gates
* We want a solution that are not affected by infrastructure, so test results are not biased by infrastructure issues

* … <!-- numbers of drivers can vary -->

## Considered Options

* Option 1: Running integration tests in a Test Container
* Option 2: Running integration tests in a PR environment
* Option 3: Using a test framework like Squadron, that enables to test in a container setup with the ability to connect to services in Azure.

* … <!-- numbers of options can vary -->

## Decision Outcome

Chosen option: "[option 1]", because [justification. e.g., only option, which meets k.o. criterion decision driver | which resolves force force | … | comes out best (see below)].

### Positive Consequences <!-- optional -->

* [e.g., improvement of quality attribute satisfaction, follow-up decisions required, …]
* …

### Negative Consequences <!-- optional -->

* [e.g., compromising quality attribute, follow-up decisions required, …]
* …

## Pros and Cons of the Options <!-- optional -->

### [option 1]

[example | description | pointer to more information | …] <!-- optional -->

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* … <!-- numbers of pros and cons can vary -->

### [option 2]

[example | description | pointer to more information | …] <!-- optional -->

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* … <!-- numbers of pros and cons can vary -->

### [option 3]

[example | description | pointer to more information | …] <!-- optional -->

* Good, because [argument a]
* Good, because [argument b]
* Bad, because [argument c]
* … <!-- numbers of pros and cons can vary -->

## Links <!-- optional -->

* [Link type] [Link to ADR] <!-- example: Refined by [ADR-0005](0005-example.md) -->
* … <!-- numbers of links can vary -->
