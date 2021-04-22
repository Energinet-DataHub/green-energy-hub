# Quality Assurance and Test in Green Energy Hub

Welcome to the introduction on how we are working with Quality Assurance and Testing in the Green Energy Hub.

## Quality Statement

As the founder of this project the danish TSO, Energinet, and Microsoft has together set the bar for the Green Energy Hub to be the best big data handling solution for green energy markets worldwide – and with this statement we all agree to work towards giving the world access to a maintainable, secure, configurable, and fast big data system.  

We consider every contribution to be thought through, described, peer reviewed and tested before accepted into our product portfolio.
This is a common responsibility, and are you, as a contributor, not able to meet these guidelines by yourself – please reach out to the community for assistance, input or help towards ensuring your contribution lives up to this quality statement.

## Quality Risk

Building a product always gives reason to consider product risk areas and the possible costs of these risks elements. In this section we would like to elaborate on how we are working with these considerations, and give inspration on how to practically address this.

In a project like this product risk contains two overall questions

1. *Are we building the right product?*
2. *Are we building the product right?*

In regards to the first question - we want to be certain that the solution is delivering us the value we need. This question can be answered by evaulating on the quality characteristics of the product. Do the functionality reach the expected level of functionality, security, performance, maintainability, usability etc. Here it is important to have all the relevant considerations outlined before starting to implement. What is the value, the targets, the expectations and the formal requirements for this product.

Writing acceptance criteria or use case scenarios before starting to develop is a great way to ensure that all considerations are taken into account during the development phase. But don't forget to try and reason for the costs of not delivering to target or expectation.

Building the right product has in earlier times been proven by a post-developing test - We want to have this mitigation before implementing, as a way of ensuring alignment of expectations. Thus the knowlegde and the learnings of this risk evaluation could also be used for automating regression test within the pipeline so we remember to re-visit our acceptance criteria throughout the delivery pipeline.

Regarding the second question on how we are delevoping our solutions. Here we talk about issues that could affect the way we are able to deliver a solution. Considering complexity, knowledge levels, tools, experience etc. is important, as these risks would very easily impact the work needed and the quality of the solution.

*Are you working on a product/solution and are you unsure of the risks - or how to mitigate certain risks you have identified? - use the community for collaboration and discussion.*

### Working with risk

One way of mitigating risks is testing - but we are all familiar with the old saying that test is a timeboxed event/a bottleneck before go live - In Green Energy we wish to ensure that test/QA is happening all the time, by everyone. We don't have a verification team - we are all responsible for mitigating risks, and ensuring quality - so rest assure that it is never too early to talk quality.

### Testing Quadrants

One way of translating risk items to actual testing could be using the testing quadrants for identifying - the relevant test types for specific areas.

### Risk Radar

A way of visualizing the migitation of risks.

## Test Approach Elements

### Working with issue templates

We are securing the quality of issue-submission by using templates for different kinds of issues – please choose the template you need, when creating an issue.

The life cycle of an issue is dynamic, and the commit team and community in general will handle the life span of an issue, according to activity for these. Please remember that all issues can be re-opened if needed.  

Please use domain repos and process tagging (e.g. change of energy supplier) on issues for enhancing traceability, transparency, and history.

### Content of Pull Requests

A Pull Request should always be,

- Related to an accepted issue
- Have signed CLA's for both author and reviewers
- Have a well-described purpose
- Adhering to established project standards (Read more on ADR's and other standards here [Here])
- Documented (The expectations to Documentation is further outlined [Here] )
- Tested (The expectations for tested is further outlined [Here] )

### Continuous integration

In each Green Energy Hub repo you will find a provided [Getting Started] and also a pre-set of CI Pipeline that contains,

- Build Scripts
- Validation of infrastructure
- Validation of code base
- Unit testing
- Code Coverage analysis and reporting
- Spell, links and lint checks
- [...]

### Test levels and types

There are a lot of frameworks and definitions of testing activities, and here we will outline the concepts used within Green Energy Hub and how we use them.

## Tools

In Green Energy Hub we need to share and delevop our ways of working - here will be of list of tools being used.

### Code Coverage

Unit test code coverage tool and KPI's.

## Test Data

Green Energy Hub would like to publish test data for use and inspiration.

## Metrics

[TBD]

## NFR

A catalog of common NFR considerations and targets [Read More](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/docs/non-functional-requirements.md).

## Documentation - Standards/structures

The Green Energy Hub project and repos are build on structures and templates.

Currently we have templates for:

- Issues
- Repo structure and standard documentation
