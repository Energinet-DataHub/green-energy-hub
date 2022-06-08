# Software Development Principles

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

Secrets includes all data that is used to gain access to a resource. Examples are but, not limited to username/passwords, connection strings, tokens. When a secret is needed, it **must** be provided from the infrastructure. It is **not to be written** within the source code or any other artefacts. Most importantly, a secret is **never to be committed to the repository**.

## 3rd party libraries

Before using a third party library an assessment must be performed with the following parameters:

- Is license is compatible with the product being developed.
- Is the component still being developed and are feature/security patches released.
- In case of open-source, is the OS community have a steady flow of PR's from many contributors.

All dependencies must be checked if they are outdated in a reoccurring interval.

## Tools that we use

This is a curated list of tools that we use to improve the quality of the software that we deliver.

### [sonarcloud.io](http://sonarcloud.io)

Sonarcloud is a static code analysis tool. It provides check for code smells, bugs, duplication and metrics for code maintainability.

This is a part of our CI pipeline. When a pull request is opened the code-change is analysed. The result is reported on the pull request as a comment. If the quality is not compliant the pull request is blocked until the quality is improved based on the feedback.

### [codecov.io](https://codecov.io)

Codecov measures the code coverage of a repository. It provides feedback when a pull request is opened. The code coverage is calculated for the pull request including the entire code base. The result is pushed to the pull request. It is possible from the pull request to see how the code change affect the overall code coverage.

### [GitGuardian](https://www.gitguardian.com)

This tool scans our source code to detect API keys, password, certificates, encryption keys and other sensitive data. The tool is invoked when a pull request is created or changes are pushed. The code is then scanned for any secrets that should not be part of the change.

If a secret is discovered admins get notified. The change then needs to get updated to remove the secret.

### [Code security - GitHub](https://docs.github.com/en/code-security)

 By hosting our sourcecode within GitHub we gain access to `Code security`. This is a collection of tools that can scan our codebase for secrets, code smells and `supply chain security`.

 Dependabot is part of `supply chain security` that analyses third party dependencies. When a package is out-of-date, a pull-request is opened with a change to upgrade the package. A developer can then accept the change and update the package.

 Besides identifying packages that are out-of-date, it also check for packages that have known vulnerabilities. This is aggregated on a dashboard that ranks the security vulnerabilities based on the severity. A team will get a banner on the repository, if a security vulnerability is identified, and they *must* take action to mitigate it.
