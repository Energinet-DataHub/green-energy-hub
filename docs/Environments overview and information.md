# Environments

Here is the total list on available resource groups per repo. These environments are all a reflection of environemnts created in the Azure portal. Links are available from individual repo.

rg - resource group

P = Production
B = Pre Production
T = Test
U = Development

## Energinet-Datahub/geh

N/A - Project information repo

## Energinet-Datahub/green-energy-hub-core

- Terraform-Module-Tests

## Energinet-Datahub/geh-terraform-modules

N/A - A collection of modules

## Energinet-Datahub/Environments-overview

- rg-Datahub-D
- rg-Datahub-T

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

 Please note, if you are contributing a PR in a repo, where you are not recknognized as 'code owner' your PR will automatically request 2 approvals from contributers stated as 'code owners'. This request is done automatically and you are able to identify code owners on your review list by a given badge.

This rule is to ensure stable environments and decentralizing the responsibility of the domain repos and product stability in all environments.
With this, it is a part of the PR gate up to the code owners to review, plan, accept and finally merge PR's, so that they are able to control stability on their main branch - and therefore also into their environments. This is as a way of ensuring domain ownership. Please respect these reviews and the process of this.

### Status checks

All PR gated status checks must be run and approved before merge approval (This is automated).

### Branch must be updated with latest version

This is to ensure that we are always merging to the latest version of main available (This is automated).

### Markdown checks and build validations

These are to ensure a common coding and CI/CD practise across repos/domains.

## CI/CD Pipeline

All repos has a build-in CI/CD infrastructure.
Please read the [Templatereadme.md](https://github.com/Energinet-DataHub/geh-shared-resources/tree/main/docs/template-readme)  for each repository for more information on how to trigger this.

## Matrices in pipeline code

Please ensure not to make any changes to the matrix definitions in the pipeline infrastructure, as this ensures that all 4 resource groups (environments) for the repo are spun up correctly, and adding to the matrix will result in creating other unsupported environments in the azure portal.

## How to use Github Actions

[TBD]

When triggering deployment to a specific environment in Github, the environment will automatically spin up with content in the Azure portal, and all functionality will be reflected in Azure Portal when the workflow is finished.

This will also happen when triggering deployment of Azure functions this workflow covers both build and deployment to Azure Portal.

Please note that it is not possible to trigger manually deployment to an environment from other branches than main.

If you need to get access to an environment for testing purposes - we refer to manual triggering a sandbox environment before requesting PR approval and adding functionality to the main branches of the domains.

## How to manually trigger sandbox setup

[TBD]

## Release strategy

[TBD]

Please note that only Development environments are not protected by a deployment rule stating that the repo owners (team based) should approve deployment to a specific environment.

## Best practise on test of infrastructure as code

[TBD]

## Changes to Terraform modules - how to

If Terraform announces large changes to their modules that needs to be prioritized upgraded a common announcement will happen, so that all contributers are made aware of the needed work to be done. In the matter of small non-blocking version changes to Terraform modules it is up to the repo-owners to decide when to update their modules in the specific repo. Please remember to inform relevant contributers if the work interfers with anything.

## How to get more information on specific repos setup

Please reach out by creating an issue in the specific repo you are requesting information on.