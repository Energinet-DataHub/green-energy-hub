# Quality Assurance and Test in Green Energy Hub

Welcome to the introduction on how we are working with Quality Assurance and Testing in the Green Energy Hub.

 This document focuses on various concepts, concerns, and approaches we consider while working on this product. How we specifically organise our approach to testing in the Danish implementation will be published as a Test Strategy at a later date.

## Quality Statement

As the founder of this project the danish TSO, Energinet, and Microsoft have together set the bar for the Green Energy Hub to be the best big data handling solution for green energy markets worldwide – and with this statement we all agree to work towards giving the world access to a maintainable, secure, configurable, and fast big data system.  
We expect every contribution to be thorougly thought through, described, peer reviewed and tested before it can be accepted into our product portfolio.
This is a common responsibility, and are you, as a contributor, not able to meet these guidelines by yourself, please reach out to the community for assistance, input or help towards ensuring your contribution lives up to this quality expectation.

## Quality Risk

Building a product always gives reason to consider quality risk areas and the possible costs of these risk elements. In this section we would like to elaborate on how we are working with these considerations, and inspire on how to practically address them.

In a product like this quality risk contains two overarching questions:

1. **Are we building the right product?**
2. **Are we building the product right?**

In regards to the first question - we want to be certain that the solution delivers the value we expect and need. This question can be answered by evaluating the quality characteristics of the product. Does the functionality reach our expected level of functionality, security, performance, maintainability, usability etc. In that way we are able to evaluate the product against the value, the targets, the expectations and the formal requirements for this product.

Writing acceptance criteria and/or use case scenarios before starting to develop is a great way to ensure that all considerations are taken into account during the development phase.

Building the right product has in earlier times been proven by a post-developing test. We want to mitigate risks during implementation, so that we are working with aligned expectations and relevant measures, so that we are able to avoid large-scaled testing bottlenecks before releasing it to end users. The knowlegde and the learnings of quality risk evaluation could also be used for inspiring automated regression tests within a pipeline setup.

Regarding if we are building the product right it is important to evaluate if the chosen architecture, technologies and tooling fits the purpose.
Does the business domain warrant a specific architecture to seperate concerns? Do we need to structure business domains in similar ways, or is it only the contracts between them that matter? All this, and more, are important considerations as they will impact the quality of the delivered solution and thus the amount of quality assurance work needed to mitigate quality risk. And most importantly, does the second question enforce the answers to the first?

*Are you working on a product/solution and are you unsure of the risks - or how to mitigate certain risks you have identified? - use the community for collaboration and discussion.*

### Working with quality risk

One way of mitigating quality risks is testing - but we are all familiar with the old saying that test is a timeboxed event/a bottleneck before go live - In Green Energy Hub we wish to ensure that test/QA is happening all the time, by everyone. We don't have a verification team - we are all responsible for mitigating risks, and ensuring quality - so rest assure that it is never too early to talk quality.

### Testing Quadrants

One way of translating quality risk items to actual test activities could be done using the Testing Quadrants.

[TODO: Add image and elaborate]

## Open Source work flows

### Green Energy Hub - Working with issue templates

We are securing the quality of issue-submission by using templates for different kinds of issues – please choose the template you need, when creating an issue.

The life cycle of an issue is dynamic, and the commit team and community in general will handle the life span of an issue, according to activity for these. Please remember that all issues can be re-opened if needed.  

Please use domain repos and process tagging (e.g. change of energy supplier) on issues for enhancing traceability, transparency, and history.

### Green Energy Hub - Content of Pull Requests

A Pull Request is intended to be,

- Related to an accepted issue
- Have signed CLA's for both author and reviewers
- Have a well-described purpose
- Adhering to established project standards (Read more on ADR's and other standards [here])
- Documented (The expectations for Documentation is further outlined [here])
- Tested (The expectations for tested is further outlined [here])

### Green Energy Hub - Continuous integration

In each Green Energy Hub repo you will find a Getting Started section and also a pre-set of CI Pipeline that contains,

- Build Scripts
- Validation of infrastructure
- Validation of code base
- Unit testing
- Code Coverage analysis and reporting
- Spell, links and lint checks
- [...]

## Green Energy Hub - Test Data

Green Energy Hub would like for all contributors to share their knowledge and artefacts used during building and testing. So if you contribute, and you already have created test data sets that cover general happy/negative flows, core calculations or scripts for populating DB etc. Please don't hesitate to share these in the specific domain you are working. In that way we support each other in always having a useful test data basis available.

## Green Energy Hub - Stubs and Mocks (Test Doubles)

We want to encourage the use of test doubles like stubs, drivers, mocks etc when working in a specific domain, where there are contracted integrations to other domains.

The Green Energy Hub is built on a domain structure and micro service setup - which causes a need for contrating our integration points between domains. As all domains are inividually developed, and with different scopes - we are not able to provide fully implemented integration points, and so we must use test doubles for simulating these integration points in our development process.

When creating such, please share the relevant information and make sure that your test double fits already agreed contracts, or remember to give notice to the community if new integration points arise, or changes to contracts are needed.

## Test activities and types

There are a lot of frameworks and definitions of testing activities, and here we will outline the concepts used within Green Energy Hub. Some of these activities are only done on indivdual forks - but please see this as an inspiration to how you as an individual contributor or organisation can organize your testing activities. Depending on your contribution or task the coverage on different test activities might differ, and some might not be applicable. We do not want you to test for the sake of testing - but share the experiences and results of testing, so that the testing is a collaborative activity, where the community are open to share concerns, ideas and new ways of thinking.

Are you using TDD or BDD or other types of coding practices please be inspirational - but respect the differences in ways of working as well.

(NB! The list is not a sequential representation of test activities)

- Peer Review (Mandatory activity in Green Energy Hub PR gate)

- Unit Test (Mandatory activity in Green Energy Hub PR gate)

- Component Test

- Infrastructure Test

- Intra Domain Integration Test

- Domain System Test

- Contract Test

- Inter-domain Integration Test

- Performance Test

- Security Test

- Automated Regression Test

- Business Process Test

## Tools

In Green Energy Hub we need to share and develop our ways of working together - here will be a list of tools used for different test activities.

- CodeCov, see code coverage

[TODO: Add remaining part of list]

## Code Coverage

We are currently using CodeCov in our PR gate to measure code coverage with the ambition of reaching 80 %. If you don't reach the target, consider why, what could increase your coverage and have you used different coverage types when designing your unit test.

### Types of Code Coverage

CodeCov uses the following types of coverage:

- Line Coverage
100% line coverage are exercised when covering at least one element of each line.

- Branch Coverage
100% Branch Coverage ensures that all possible outcomes of a decision are tested at least once

NB! Always compare the result of unit test to expected result.

## Non-functional requirements

A catalog of common non-functional requirements can be found [here](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/docs/non-functional-requirements.md).

## Documentation - Standards/structures

The Green Energy Hub project and repos are build on structures and templates.

Currently we have templates for:

- Issues
- Bug reports
- Repo structure and standard documentation

When writing documentation for how a process work these things are essential to include:

- What is the purpose of this functionality? What problem does it solve?
- How does it fit in the grand scheme of things
- How does it work? What is the expected input, and what can we expect as output?
- How is it architecturally structured? What technologies are used and why?

When writing technical documentation for features/functionality following topics can be relevant:

- Name/version/Service provided
- Overall description and relation to the overall architectural landscape and functionality
- Terms/Definitions used in document and solution
- Acceptance criteria for the product
- Preconditions/mandatory requirements of the product
- Non-functional requirements
- Security notices. General description of security observations and needs
- Contract relations
- Test doubles used
- Test data used
- Validation rules
- Expected result
- Examples
- Error codes
- Debugging notes
- Installation guides
