# How to test integration points within domains prior to deploying to DEV (U) environment

* Status: [Partly decided]
* Deciders: [@Sondergaard, @Energinet-Datahub/mightyducks]
* Date: [2021-09-01 when the decision was last updated] <!-- optional -->

Technical Story: [description | ticket/issue URL] <!-- optional -->

## Context and Problem Statement

How can we leverage integration testing, so that it

* Increases our software confidence
* Supports an efficient development process
* Enables a fast and reliable feedback loop
* Guards the DEV environment from becoming unstable and thereby increases DEV availability
* Enables us to ensure connectivity and data correctness

To ensure that integrations with external dependencies (storage, communication bus etc.) are set up correctly and the application logic works as expected prior to deploying to DEV (U), we need a framework that enables us to run integration tests both on localhost and in the continuous integration pipelines.

## Decision Drivers

* We need an integration test solution that fits a microservice architecture as well as the practices of DevOps
* The solution must solve the problem with cloud components that cannot be emulated, specifically for local testing efforts
* We emphasize failing fast, having quick and reliable feedback loops and DEV environment stability and availability
* We want a solution that does not require a full domain setup deployed as a cloud environment
* The solution must be easy to set up and tear down as part of PR gate or deployment gates
* We want a solution that are not affected by infrastructure, so test results are not biased by infrastructure issues

Following use cases were identified, that needs to be covered by this test framework:

* ### Triggering job in Ccomponents

Being able to trigger, read status and verify output from the component.

* ### Integration to Event Hub

Verifying event is generated in event hub and that it contains expected data.

* ### Integration to a ServiceBus

Publishing a ChargeCommandReceived event from an azure function (MessageReceiver). Receiving this event from yet another az function (ChargeCommandReceiver).
The test will be initiated by a HTTP POST call with JSON content.
(No seed data required)

* ### Databricks integration to a DeltaLake

Write access - storing a time series in the DeltaLake (from Databricks)
It is assumed that the test will be initiated by placing valid test data on an EventHub from which Databricks will fetch the time series.
(No seed data required)

* ### Integration to an Sql database

Read/Write access for reading and storing a record from an application.
The database must be seeded with appropiate test data prior to running the application code.

## Considered Options

* Option 1: Running integration tests in a Test Container
* Option 2: Running integration tests in a PR environment
* Option 3: Using a test framework like Squadron, that enables to test in a container setup with the ability to connect to services in Azure.

* … <!-- numbers of options can vary -->

## Decision Outcome

For use cases regarding components like eventhub, service bus and Sql database we have implemented a Sqaudron based solution, where it is possible both local-hosted and in CI pipeline to ensure application-to-service  connections.

***Regarding the use case for DeltaLake - it has not yet been finalized on which solution is most valuable, and therefore the decision to cover this is still pending.***

### Positive Consequences <!-- optional -->

* Besides testing component connections locally Squadron allows verification that components are connecting correctly to storage instances by creating a containerized setup where both application layer and service is connected in an infrastructure setup (excluding Terraform - further outline on this under negative consequences).

* The solution is implemented to run both locally and in CI setup.

* This adds to  even more risk elements to be covered prior to deployment, and mitigiates risks that essentially could cause instability and issues on environments and further out in the CD pipeline, which would enhance the complete lead time for functionality delivery.

### Negative Consequences <!-- optional -->

One risk area that has been mentioned for this strategy for testing prior to dev is that the solution does not cover testing of compliance and quality of the solution deployed with correct Terraform modules. This risk is also be present in a test container setup - but could be mitigated by a PR environment solution. We are already using a PR environment solution to ensure test on Terraform and infrastructure - and it was also tested as a solution for intra-domain integration testing. This is can be done, but depending on the testing discipline a very expensive solution, and is therefore not the approach we have choosed.

Given the isolated testing on components and infrastructure, we are mitigating the risk of this issue, by having the possibility of testing services in the PR environment - but given price plans etc the PR environment is triggered in PR gate and torn down immedialtely after given tests have been performed, and should not be used for debugging in the PR gate over a longer period of time.

You can find an overview on the testing activities across environments [HERE](https://github.com/Energinet-DataHub/dh3-documentation/blob/fdbcae1774d7bd41d32108516ebfadfae74e15e8/DOCS/images/test-strategy-visual%202021-0830.png)

***Mitigation of DEV (U) instabilities due to this risk, should be handled - please create an issue if risk occurs***
