# Integration tests

Integration testing (sometimes called integration and testing, abbreviated I&T) is the phase in software testing in which individual software modules or micro services are combined and tested as a group.
Integration tests are important, but not only part of quality assurance procedures.
Usually, they are ran after unit testing and before validation testing.
As of now, there are two workflows related to Integration testing - deployment of [Integration Testing Infra](../../.github/workflows/databricks-integration-testing-infra-cd.yml) and [Integration Testing](../../.github/workflows/databricks-it.yml) workflow.
Also sample [Streaming Integration Test](../../src/integration-test/streaming-test.py) has been created for Green Energy Hub project.
It covers EventHub, DataBricks, CosmosDB integration points and helps to validate changes in DataBricks Jobs.

Integration tests should run in separate environment, similar to production one, but with less compute power.
It can be deployed (or turned on) each time you want to run integration tests, or it can run continuously, depending on how frequently you are scheduling integration tests.
One of the key question, that needs to be answered, is when to run Integration Tests? And the short answer is: you should try to run them for each PR, so the main branch will be cleaner and release process will be easier.
But, there is a number of constraints (time, price, etc...) you may face, that will change your decision to run Integration Tests on a schedule or even during release procedure.

Lets have a look at approaches you may follow to plan Integration Testing execution.

## Integration tests as part of CI

Integration tests can be included in CI pipeline, so they will run for every PR before merging to main branch.
Definitely, this is the best way to ensure you are merging clean and working code in main, but from the other side, it is cost and time consuming approach.
Likely, your integration tests deployment and execution procedure will take much more time (tens of minutes), than unit tests normally take.
During rapid development, it can become a problem: in some active PRs, each commit will trigger integration tests re-run. This may slow down the development and PR approval process.
Another concern is price for continuously running environment. If you want to speedup integration tests workflow execution, likely you will keep standalone hot environment that will be used to run integration tests.
More over, during active development, there will be situations, when there is multiple active PRs required to run integration tests.
It means, that at some point, 2 - 3 or even more workflows will be executed in parallel.
To be able to execute them, you should have multiple isolated slots in your environment, or even have 2 - 3 or more separate environments and additional logic, that will distribute integration tests across existing environments.
Also, your workflow should filter changes in PR to make sure you are running integration tests which cover integration points related to the changed code.
Implementing this will definitely add complexity in your workflow and price for the integration testing environment.

## Integration tests as part of nightly builds

This is a widely used way to control your development process, but in this case, the whole DevOps process should be optimized for this.
There is a couple of practices you may follow to make dev process comfortable with respect to QA:

1. Your main branch should contain a code of last stable release. Only critical bugs, that can't wait for the next release, should be solved in main branch.

2. Development of a new release should be done in other branch (like 'v2', 'dev', etc). It will contain latest, but not yet stable version of your application.
This branch is normally used to run nightly builds.

3. Release process should be changed (schedule based releases or manual release process)

There are alternative approaches to first points from above, for instance using  git tags, but the general idea is to keep production code and development code separated.

In this case, to assure certain level of code quality and prevent portion of the bugs to be pushed to dev branch, you can still run fast checks (static checks and unit tests) as part of your CI.
But in order to keep your dev branch up to date with code, that can be deployed and run without major issues, you could setup a Nightly Build workflow, that will be triggered at night and execute unit, integration and other automated tests.
The result of a nightly build can be injected in root README.md using [badges](https://github.com/badges/shields).

A good approach will be to have a person or a group of people, who will be notified about failed builds.
They should be able to quickly triage a problem, find a root cause and engineer, who can apply hot fix.

Using this approach you introduce less complexity and reduce price for environment, as there is no need to keep it continuously running.
During Nightly Build there is no time rush to run all checks and tests quickly, so during this process a brand new environment can be created and destroyed after tests will be finished.
More over, you can control test sequence in your workflow, so "parallel execution" problem will be simplified.

From other hand side, it will require more human resources to keep main branch up to date with working version of application.
More over, during a day, a "buggy" PR can be a root for other changes between builds.
In this case investigation and resolution can touch multiple engineering groups and contributors.
Taking into account, that OSS project can have hundreds of contributors without maintenance responsibility, issues detected in nightly build can be a reason of delaying releases.
Partially it can be resolved by strict code review process, where code owners should take care of potential integration issues.
