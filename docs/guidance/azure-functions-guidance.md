# Deploying Azure Functions

Currently Green Energy Hub repository contains two workflows related to deployment of Azure Functions Apps code.
Namely [main-pr-gate.yml](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/.github/workflows/main-pr-gate.yml) and [postoffice-http-trigger-cd.yml](../../.github/workflows/postoffice-http-trigger-cd.yml).
(There is also third workflow [deploy-function.yml](../../.github/workflows/deploy-function.yml) used just as an example and can be erased once the final deployment approach for Azure Functions is in place)

These two workflows deliver slightly different results. First let's discuss the details of both of these workflows and afterwards we will share few recommendation or possible ways to move forward when delivering final deployment strategy for Azure Function Apps within the solution.

## Main PR Gate

[main-pr-gate.yml](../../.github/workflows/main-pr-gate.yml)  workflow builds and tests Azure Function Apps within the solution and it uploads Azure Function Apps packages into Github Action Artifacts store from where it can get picked up by subsequent jobs.

This workflow is targeting multiple Azure Function App projects by defining separate job per Function App project. Additionally, there is license check job invoked to check all the projects file for presence of license information using [kt3k/license_checker@v1.0.3 action](https://github.com/kt3k/license_checker).  

Azure Function App build and test jobs start with code checkout.
Then there is .Net Core setup step invoked which installs .Net Core onto the build agent and subsequently [Build and Test](../../.github/actions/dotnet-build-and-test) custom action is called, which as the name says builds and also unit tests the Function App project.
Subsequently it uses Upload Artifact step to upload built package to the artifacts store.
In order to not to rerun deployment of each function on each workflow run, but rather do it only in case particular Azure Function App was changed there is [fkirc/skip-duplicate-actions](https://github.com/fkirc/skip-duplicate-actions) action used within the jobs.

```yml
  - name: Soap validation skip check
    id: soap_validation_check
    uses: fkirc/skip-duplicate-actions@v1.4.0
    with:
      github_token: ${{ github.token }}
      paths: '["samples/energinet/soap-schema-validator/**",
        ".github/workflows/main-pr-gate.yml", ".editorconfig",
        "Directory.Build.props"]'
```

It allows to check if particular files were changed in the commit which triggered the workflow run. This information can be then used to conditionally skip the steps of the jobs, so that function is not redeployed if it is not needed.

```yml
if: ${{ steps.soap_validation_check.outputs.should_skip != 'true' }}
```

This way you can run Function App job only in case some code changes were introduced to particular Function App.
However, there is one situation, when the Function App jobs will be rerun even in case there might be no need to do it. It is in case there is change to the workflow definition introduced.
As there is no simple way to check, if changes brought to the workflow influenced how all the Function Apps should be built and tested, the simplest solution is to rerun all the Function App jobs in the workflow definition.
One way around this is to create separate workflow files for each Function App job.

If you navigate to the [main-pr-gate.yml](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/.github/workflows/main-pr-gate.yml) workflow definition you might notice, there is a lot of code duplication.
Basically, all Function App jobs have the same code only with different parameters. Once there is support for job templates in Github Actions this workflow can be simplified substantially.

## Postoffice Http Trigger CD ([postoffice-http-trigger-cd.yml](../../.github/workflows/postoffice-http-trigger-cd.yml) based on deploy-function.yml sample)

[postoffice-http-trigger-cd.yml](../../.github/workflows/postoffice-http-trigger-cd.yml) workflow is quite simple, and its sole responsibility is to create function package and deploy it to pre-deployed Azure Function App resource in Azure (this should be created by running [infra-cd.yml](../../.github/workflows/infra-cd.yml) workflow).
This workflow is not making use of *Build and Publish custom action*, however as further improvement it can be introduced and swapped for *Build package step* to include unit testing. So, the workflow is capable to deploy the Function App package it needs publish profile definition, which is obtained using Azure CLI:

```yml
- name: Get Function Publish Profile
  id: get-publish-profile
  run: |
    publish_profile=$(az webapp deployment list-publishing-profiles --name ${{   env.AZURE_FUNCTIONAPP_NAME }} --resource-group ${{ env.RESOURCE_GROUP_NAME }} --subscription ${{ env.ARM_SUBSCRIPTION_ID }} --xml)
   echo "::set-output name=publish-profile::${publish_profile}"
```

Publish profile needs to be obtained in xml format, thus using --xml flag. Publish profile is then used in official [Azure/functions-action](https://github.com/Azure/functions-action) which deploys the Function App package:

```yml
- name: 'Run Azure Functions Action'
  uses: Azure/functions-action@v1
  with:
    app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
    package: '${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}/output'
    publish-profile: ${{ steps.get-publish-profile.outputs.publish-profile }}
```

## Recommendations

Our recommendation for building the Function App deployment workflow is either to modify *main-pr-gate* workflow to publish app packages directly to Azure using approach from *postoffice-http-trigger-cd* workflow, or if you want to overcome possible deployment overhead when modifying workflow file, create separate deployment workflow for each Function App.
In case you create separate workflow for each Function App you can shift paths filtering to workflow trigger settings and get rid of using *fkirc/skip-duplicate-actions*. **Note!** When doing so, be aware of know [issue](https://github.community/t/dealing-with-checks-enforced-by-branch-protection-but-skipped-by-path-filter/18451) with branch protection rules using workflow with path filters.
