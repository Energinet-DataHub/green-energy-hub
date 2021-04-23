# Quality Assurance and Test in Green Energy Hub

Welcome to the introduction on how we are working with Quality Assurance and Testing in the Green Energy Hub.

## Quality Statement

As the founder of this project the danish TSO, Energinet, and Microsoft has together set the bar for the Green Energy Hub to be the best big data handling solution for green energy markets worldwide – and with this statement we all agree to work towards giving the world access to a maintainable, secure, configurable, and fast big data system.  
We consider every contribution to be thought through, described, peer reviewed and tested before accepted into our product portfolio.
This is a common responsibility, and are you, as a contributor, not able to meet these guidelines by yourself, please reach out to the community for assistance, input or help towards ensuring your contribution lives up to this quality statement.

## Quality Risk

Building a product always gives reason to consider product risk areas and the possible costs of these risks elements. In this section we would like to elaborate on how we are working with these considerations, and give inspration on how to practically address this.

In a project like this product risk contains two overall questions

1. *Are we building the right product?*
2. *Are we building the product right?*

In regards to the first question - we want to be certain that the solution is delivering us the value we need. This question can be answered by evaulating on the quality characteristics of the product. Do the functionality reach the expected level of functionality, security, performance, maintainability, usability etc. Here it is important to have all the relevant considerations outlined before starting to implement. What is the value, the targets, the expectations and the formal requirements for this product.

Writing acceptance criteria and/or use case scenarios before starting to develop is a great way to ensure that all considerations are taken into account during the development phase. But don't forget to try and reason for the costs of not delivering to target or expectation.

Building the right product has in earlier times been proven by a post-developing test - We want to have this mitigation before implementing, as a way of ensuring alignment of expectations. Thus the knowlegde and the learnings of this risk evaluation could also be used for automating regression test within the pipeline so we remember to re-visit our acceptance criteria throughout the delivery pipeline.

Regarding the second question on how we are delevoping our solutions. Here we talk about issues that could affect the way we are able to deliver a solution. Considering complexity, knowledge levels, tools, experience etc. is important, as these risks would very easily impact the work needed and the quality of the solution.

*Are you working on a product/solution and are you unsure of the risks - or how to mitigate certain risks you have identified? - use the community for collaboration and discussion.*

### Working with risk

One way of mitigating risks is testing - but we are all familiar with the old saying that test is a timeboxed event/a bottleneck before go live - In Green Energy we wish to ensure that test/QA is happening all the time, by everyone. We don't have a verification team - we are all responsible for mitigating risks, and ensuring quality - so rest assure that it is never too early to talk quality.

### Testing Quadrants

One way of translating risk items to actual testing could be using the testing quadrants for identifying - the relevant test types for specific areas.

[image]

### Risk Radar

A way of visualizing the migitation of risks.

[TBW]

## Open Source and Test Approach Elements

This section is meant as inspiration for all contributors to Green Energy Hub. Whether you're working on forked instance or delivering into the community - we would like to share the thoughts, and choices we have made to ensure QA and testing activities througout our work with Green Energy Hub. Some of the choices and activities are done to ensure an alignment towards our community and Green Energy Hub as a whole, and some activities are done in the Energinet setup.

### Green Energy Hub - Working with issue templates

We are securing the quality of issue-submission by using templates for different kinds of issues – please choose the template you need, when creating an issue.

The life cycle of an issue is dynamic, and the commit team and community in general will handle the life span of an issue, according to activity for these. Please remember that all issues can be re-opened if needed.  

Please use domain repos and process tagging (e.g. change of energy supplier) on issues for enhancing traceability, transparency, and history.

### Green Energy Hub - Content of Pull Requests

A Pull Request should always be,

- Related to an accepted issue
- Have signed CLA's for both author and reviewers
- Have a well-described purpose
- Adhering to established project standards (Read more on ADR's and other standards here [Here])
- Documented (The expectations to Documentation is further outlined [Here] )
- Tested (The expectations for tested is further outlined [Here] )

### Green Energy Hub - Continuous integration

In each Green Energy Hub repo you will find a provided [Getting Started] and also a pre-set of CI Pipeline that contains,

- Build Scripts
- Validation of infrastructure
- Validation of code base
- Unit testing
- Code Coverage analysis and reporting
- Spell, links and lint checks
- [...]

### Green Energy Hub - Test Data

Green Energy Hub would like for all contributors to share their knowledgde and artefacts used during building and testing. So if you contribute, and you already have created test data sets that covers general happy/negative flows, core calculations or scripts for populating DB etc. Please don't hesitate to share these in the specific domain you are working. In that way we support each other in always having a basis of test data to use.

### Green Energy Hub - Stubs and Mocks (Test Doubles)

We want to encourage the use of test doubles like stubs, drivers, mocks etc when working in a specific domain, where there are contracted integrations to other domains.

The Green Energy Hub is build on a domain structure and micro service setup - which causes a need for contrating our integration points between domains. As all domains are inividual developed, and with different scope - we are not able to provide fully implemented integrations points, and so we must use test doubles for simulating these integration points in our build process.

When creating such, please share the relevant information and make sure that your test double fits already agreed contracts, or remember to give notice to the community if new integration points arises, or changes to contracts are needed.

### Test activities and types

There are a lot of frameworks and definitions of testing activities, and here we will outline the concepts used within Green Energy Hub and how we use them. Some of these activities are only done on indivdual forks - but please see this as an inspiration to how you as an individual contributor or organisation can organize your quality activities. Depending on your contribution or task the coverage on different test activities might differ, and some might not be applicable. We do not want you to test for the sake of testing - but share the experiences and results of testing, so that the testing is a collaborative activity, where the community are open to share concerns, ideas and new ways of thinking.

Are you using TTD or BDD or other types of code practises please be inspirational - but respect the difference in ways of working as well.

- Peer Review (Mandatory activity in Green Energy Hub PR gate)

- Unit/Component test (Mandatory activity in Green Energy Hub PR gate)

- Internal Domain integration test

- Infrastructure testing

- Contract test

- Inter-domain integration test

- Performance testing

- Security test

- Automated regression test

- Business flow test

## Tools

In Green Energy Hub we need to share and delevop our ways of working - here will be of list of tools being used.

### Code Coverage

We are currently using Coverlet in our PR gate to ensure a 80% unit test code coverage. This is not blocking for PR's - yet - but we would like all to consider how to gain quality where possible, and also be true to the expectation of challenging your own work and others. If you don't reach the target, consider why, what could increase your coverage and have you used different coverage types when designing your unit test.

#### Types of Code Coverage

- Line Coverage
100% line coverage are exercised when covering at least one element of each line.

- Statement Coverage
100% statement coverage is achieved when covering all statements in the code.

- Decision Coverage
100% Decision Coverage are exercised when all possible outcomes of decision statements are covered

- Path Coverage
Same approach as decision coverage, whereas decision coverage only test for all possible outcomes are functional - path coverage are to determine how the business logic are functional across multiple decisions. Path coverage will automatically require decisions being tested multiple times in different paths, and the test coverage for decision coverage will exceed 100% with path coverage.

NB! Always compare the result of unit test to expected result

## Metrics

[TBD]

## NFR

A catalog of common NFR considerations and targets [Read More](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/docs/non-functional-requirements.md).

## Documentation - Standards/structures

The Green Energy Hub project and repos are build on structures and templates.

Currently we have templates for:

- Issues
- Repo structure and standard documentation
