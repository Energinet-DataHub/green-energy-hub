# Software Development Model

After reading this document you will have insight in how we develop secure software with built-in quality. It will cover our branching model, password management, use of 3rd party libraries.

## Branching model

Developers collaborate on code in a single branch called **main**. The code in **main branch** is what is in **production**. When a change is integrated into the **main branch**, it is built, tested, verified and pushed to **all environments**.

The main branch is protected and prevents any changes being pushed directly. To have a change integrated into main, a new branch is created from the main branch **HEAD**. This new branch should be **short-lived**, and focus only on **a single change**. By keeping the change **small and focused** it becomes easier to do a **peer-review** of the change.

## Continuously integration and deployment

With the branching model selected we are able to do continuously integration and deployment (CI/CD). The integration pipeline builds, tests and verifies the codebase before it is deployed. Our integration pipeline is focused on **shift left**. This means that we **test and verify** the change before is it being integrated into the main branch.

When a change is integrated into main a code review is mandatory. The smaller and focused the change is, the easier it is to perform a good code review. Everyone with access to the repository can create a pull-request for review, but only code-owners can approve/accept the change. The reason for this is that a group of developers are responsible for keeping the codebase consistent.
<!--
# Software Development Principals

## Working with source code

Developers collaborate on code in a single branch called **main**. Branches are **short-lived** and integrated into **main** with a **pull-request**. A pull-request **must be approved** by **another developer**.

## Integrating changes

Pull-requests are used to integrate changes to the existing code. When a pull-request is created it **must** fullfil some requirements.

When creating a pull-request it is expected to include a title that contains the essence of the change. In the description you must provide all the details that can help doing the review of the change. That includes references to the original task/issue.

Eg:

> - Which testes were affected by the change?
> - Is it change of behavior or new functionality?
> - Would this change affect performance?

Reference to issue
Hvem kan approve kode rettelser

2 faktor auth til github

Håndtering af passwords
-->

<!-- ## Continuously integration and deployment

All changes **must** be **integrated and deployed** with 100% automation. As part of the **pull-request** the change **must** pass all checks.

### Built-in quality

Artifacts that are deployed automatically must have a high level of quality. This is achieved with testing, and tools that can inspect the solution and spot pitfalls.

First and foremost the solution must be able to build on the CI server. If this is not possible the **pull-request is blocked**.



- artifacts can be built from the repository
- all tests pass

Our practices of developing software **must** support the **deployment strategy**. Our strategy is to **continuously deploy small and incremental** changes to all environments. With this approach it is **not possible** to have any **manually gates** when deploying software changes. All changes **must** be verified, tested and deployed with automation.

It is desirable to have **short-lived branches** that is merged into main. The changes that goes into a *pull request* should be **related and small**. This ensures that other developers are setup for success when performing a **code review** of the change. It should **not be possible** to get changes into the codebase **without a code/peer review**.

- SonarCloud - code smells
- WhiteSource Bolt - dependency vulnerability
- Dependabot
- dependency-check
 -->
