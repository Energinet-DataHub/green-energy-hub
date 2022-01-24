# Software Development Principals

After reading this document you will have insight in how we develop software with built-in quality. It will cover our branching model, secret management, use of 3rd party libraries.

## Branching model

Developers collaborate on code in a single branch called **main**. The code in **main branch** is what is in **production**. When a change is integrated into the **main branch**, it is built, tested, verified and pushed to **all environments**.

The main branch is protected and prevents any changes being pushed directly. To have a change integrated into main, a new branch is created from the main branch **HEAD**. This new branch should be **short-lived**, and focus only on **a single change**. By keeping the change **small and focused** it becomes easier to do a **peer-review** of the change.

## Continuously integration and deployment

With the branching model selected we are able to do continuously integration and deployment (CI/CD). The integration pipeline builds, tests and verifies the codebase before it is deployed. Our integration pipeline is focused on identifying issues as early as possible. This means that we **test and verify** the change before is it being integrated into the main branch.

When a change is integrated into main a code review is mandatory. The smaller and focused the change is, the easier it is to perform a good code review. Everyone **with access** to the repository can create a pull-request for review, but **only code-owners can approve/accept** the change.

Before a pull-request can be merged it needs to pass quality gates. This includes building the solution, exercise all associated tests and perform **static code analysis**, **vulnerability scanning**, **code quality scanning**.

When **all quality gates are passed** the artefact is deployed. The deployment starts with the development environment and gradually deploy to test, pre-prod and production. After an environment is updated a health-check can be performed to ensure that everything is running as expected. If the health-check is OK, then deployment can advance to the next environment.

## Secret management

Secrets covers all data that is used to gain access to a resource. Examples are but not limited to username/passwords, connection strings, tokens. When a secret is needed it **must** be provided from the environment and **not placed** within the source code or any other artifacts. Most importanly, a secret must **not be commited to the repository**.

## 3rd party libraries

Before using a third party library an assessment must be performed. In the assessment it must be consider if the license is compatible with the product being developed. Is the component still being developed and are feature/security patches released. In case of open-source, is it then a one-man-army maintaining the component or is maintained by many contributors.

All dependencies must be checked if they are outdated in a reoccurring interval.
