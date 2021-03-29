# Environments

Here is the total list on available resource groups per repo. These environments are all a reflection of environments created in the Azure portal. Links are available from individual repo.

rg - resource group

P = Production
B = Pre Production
T = Test
U = Development

## Energinet-Datahub/geh

Project information repo

## Energinet-Datahub/geh-terraform-modules

 A collection of Terraform modules

## Energinet-Datahub/Environments-overview

This repository will collect changelogs and readme files from domain respositories

## Energinet-Datahub/geh-shared-resources

- rg-Datahub-SharedResources-P
- rg-Datahub-SharedResources-B
- rg-Datahub-SharedResources-T
- rg-Datahub-SharedResources-U

## Energinet-Datahub/geh-api-gateway

- rg-Datahub-ApiGateway-P
- rg-Datahub-ApiGateway-B
- rg-Datahub-ApiGateway-T
- rg-Datahub-ApiGateway-U

## Energinet-Datahub/geh-post-office

- rg-Datahub-PostOffice-P
- rg-Datahub-PostOffice-B
- rg-Datahub-PostOffice-T
- rg-Datahub-PostOffice-U

## Energinet-Datahub/geh-metering-point

- rg-Datahub-MeteringPoint-P
- rg-Datahub-MeteringPoint-B
- rg-Datahub-MeteringPoint-T
- rg-Datahub-MeteringPoint-U

## Energinet-Datahub/geh-timeseries

- rg-Datahub-TimeSeries-P
- rg-Datahub-TimeSeries-B
- rg-Datahub-TimeSeries-T
- rg-Datahub-TimeSeries-U

## Energinet-Datahub/geh-validation-reports

- rg-DataHub-ValidationReports-P
- rg-DataHub-ValidationReports-B
- rg-DataHub-ValidationReports-T
- rg-DataHub-ValidationReports-U

## Energinet-Datahub/geh-aggregations

- rg-DataHub-Aggregations-P
- rg-DataHub-Aggregations-B
- rg-DataHub-Aggregations-T
- rg-DataHub-Aggregations-U

## Energinet-Datahub/geh-market-roles

- rg-DataHub-MarketRoles-P
- rg-DataHub-MarketRoles-B
- rg-DataHub-MarketRoles-T
- rg-DataHub-MarketRoles-U

## Energinet-Datahub/geh-charges

- rg-DataHub-Charges-P
- rg-DataHub-Charges-B
- rg-DataHub-Charges-T
- rg-DataHub-Charges-U

## Protection rules

### Code Owners

 Please note, if you are contributing a PR in a repo, where you are not recognized as 'code owner' your PR will automatically request 2 approvals from contributors stated as 'code owners'. This request is done automatically and you are able to identify code owners on your review list by a given badge.

This rule is to ensure stable environments and decentralizing the responsibility of the domain repositories and product stability in all environments.
With this, it is a part of the PR gate up to the code owners to review, plan, accept and finally merge PR's, so that they are able to control stability on their main branch - and therefore also into their environments. This is as a way of ensuring domain ownership. Please respect these reviews and the process of this.

### Other branch protection rules

- Require pull request reviews before merging

- Dismiss stale pull request approvals when new commits are pushed

- Require status checks to pass before merging

- Require branches to be up to date before merging

## CI/CD Pipeline

### Infrastructure CI/CD

When triggering deployments to a any environment through Github, the resources will automatically be created inside the corresponding resource group in Azure.

Please read the template-readme.md located in each domain repository at the path docs/template-readme.md

### Azure Function CI/CD

Triggering a function CD will automatically build and deploy your application to the given resource in Azure.

## Matrixes in pipeline code

Please ensure not to make any changes to the matrix definitions in the CI/CD pipelines, as this ensures that the connection (SPN) to the corresponding environments are kept intact.
Also do not add any new matrixes, since this will create new and unsupported environments in github.

## How to use Github Actions

[TBD]

Please note that it is not possible to trigger deployments to any environment from other branches than main.

If you need to get access to an environment for testing purposes - we refer to manual triggering a sandbox environment before requesting PR approval and adding functionality to the main branches of the domains.

## How to manually trigger sandbox setup

[TBD]

## Release strategy

[TBD]

Please note that only Development environments are not protected by a deployment rule stating that the repo owners (currently team based) should approve deployment to a specific environment.

## Best practice on test of infrastructure as code

[TBD]

## Changes to Terraform modules - how to

If Hashicorp announces large changes to their Terraform providers, which will require updates to our modules (geh-terraform-modules).

The platform team will handle these updates, and distribute a message to the teams once the update has been done.

Each domain will be using the Terraform modules with a release tag, meaning that any changes made by the platform team, will not break any running code.

Please be aware, that we can in case of security issues or similar, require you to update your module usage to the latest running version.

## How to get more information on specific repositories setup

Please reach out by creating an issue in the specific repo you are requesting information on.
