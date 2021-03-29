# Leverage Integration Testing using Test Containers

* Status: Decided
* Deciders: @prtandrup, @renetnielsen, @erduna, @martinfhansen, @asq-en, @Mech0z
* Date: 2021-02-22

Technical Story: ADO#90913 Decide on strategy for coded integration tests with external shared resources

Also, the creation of this ADR was initiated as a result of Workshop#2 'Working with micro services' on 15 February 2021.

## Context and Problem Statement

How can we leverage integration testing, so that it

* increases our software confidence
* supports an efficient development process
* enables a fast and reliable feedback loop
* guards the DEV environment from becoming unstable and thereby increases DEV availability

And how to overcome the problem with cloud components that cannot be emulated, like Event Hubs?

## Decision Drivers <!-- optional -->

The decision will be based on the following drivers:

* We need an integration test solution that fits a microservice architecture as well as the practices of DevOps.
* The solution must solve the problem with cloud components that cannot be emulated, specifically for local testing efforts.
* We emphasize failing fast, having quick and reliable feedback loops and DEV environment stability and availability.
* Better and more cost-efficient solutions seem to exist compared to our initial integration test solution where necessary DEV resources are booked for testing and then released after test run.
In the long run, the current solution could potentially impose high maintenance costs due to the level of complexity.

## Considered Options

* Option 1: Running integration tests in a Test Container
* Option 2: Running integration tests in a Sandbox environment
* Option 3: Continue with our initial integration test solution (Booking and releasing resources in DEV)

## Decision Outcome

Chosen option: Option 1.

Please note that this is a strategical decision that entails doing a Proof-of-Concept to explore the technical capabilities of test containers in the DataHub context.
Further explanation and justification why this option was chosen can be read below.

### Test Containers

The Test Container concept is a way to bundle what is usually containerized for running an application or in this case microservice(s) together with what is required for executing given tests in different computing environments, like test scripts, test data, frameworks, settings files, etc.
This concept leverages the known benefits of containerization and enables execution of tests in a portable and isolated manner.

This option was chosen, because:

* It provides a way to run integration tests locally, during CI and in CD, hence it is portable.
* It provides a way to run tests isolated and thus imposes no risk of false test results due to other people or applications using the system under test at the same time tests are running.
* It has no impact on DEV availability, but it may prove to increase DEV stability as integration issues may be detected before the microservice enters the DEV environment hosting the DataHub application.
* It enables test data controllability as the container itself includes the relevant test data and does not have to use data sources where data might be in an unforeseen state.
* It is a known approach for integration testing, please see this example from the Java community: <https://www.testcontainers.org/>
* It is cost-efficient as the resources are built when needed and teared down after test completion.
* It does not block the usage of an environment or resources like the current integration test solution does.

#### Intention and next steps

From a DataHub perspective, the intention is to use test containers for integration testing at the PR gate. This means that as part of the PR gate checks, a Test Container with the microservice(s) in question, it's data, dependencies, etc are built and then the integration tests are executed.
If the integration tests fail, the PR cannot be merged into the main branch.

The test container is expectedly created on the DEV environment isolated from the DataHub application also hosted on DEV.

Also, with the planned segregation into domains, documented in [ADR-0005](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/docs/adr/0005-Segregate-the-system-into-smaller-chunks.md), the test container approach seems both well-suited and feasible. Based on this, a Proof-of-Concept must be made to evaluate the test container's suitability for the DataHub context. A Proof-of-Concept assignment has been created with the PostOffice domain as candidate. This assignment is planned to be up for grabs at next Program Increment planning, (ADO#113368).

After the Proof-of-Concept results have been evaluated a new ADR is to be created justifying either the adoption or rejection of Test Containers for integration testing.

### Positive Consequences <!-- optional -->

Given the Proof-of-Concept assignment results in adoption of test containers, the positive benefits may be:

* A generic approach to integration testing which may easily be applied across teams.
* An approach that works for local development as well as in CI/CD and it can handle cloud components without emulation options.
* Increased stability and availability of the DataHub application hosted on DEV, as the microservice(s) is integration tested prior to its' entry, thus reducing the number of integration issues that would have otherwise been deployed to the DataHub application.
* The DataHub application on DEV may be freed-up for other testing purposes where an example could be doing regression testing of a domain before the domain may move forward to the next environment.

### Negative Consequences <!-- optional -->

* Adoption of the test container approach means that existing integration tests need to be refactored. However the potential value of switching to test containers seem to outweight the potential maintenance burden that follows the current integration test solution.

## Pros and Cons of the Options <!-- optional -->

### [option 1] Running integration tests in a Test Container

* Please read the Decision Outcome section.

### [option 2] Running integration tests in a Sandbox environment

This option meant running integration tests on either one or more microservices, a domain or a complete DataHub application in an environment prior to DEV, e.g. Sandbox.

* Good, because it shares some of the same benefits as option 1; it may reduce the number of integration issues being deployed to DEV and it provides a quick feedback loop.
* Bad, because it potentially provides added work when it comes to being in control of your test data.
* Bad, because it is unclear about the responsibility of making the system available for testing.
* Bad, because sharing the same environment/resources may cause delays when multiple PRs are at the PR gate.
* Bad, because there is not a clear approach to handle cloud components without emulation options.
Potentially, the same Event Hub resource could be used simultaneously for local tests while integration tests are being executed at the PR gate, which may give false test results.
Potentially, the same Event Hub resource could be used simultaneously for local tests while integration tests are being executed at the PR gate, which may give false test results.
* Bad, because if this means having a running application in sandbox, the associated cost are expected to be higher than option 1.
* Bad, because it is not portable.

### [option 3] Continue with our initial integration test solution (the booking of resources in DEV)

This options level of complexity and potentially high maintenance cost as the system evolves is enough to search for other Integration Testing solutions.
