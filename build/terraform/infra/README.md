# Infra

## Developer Experience

It is possible to deploy infrastructure from localhost.

For installation and learning resources on Terraform refer to [terraform.io](https://www.terraform.io).

The steps below solves the problem of otherwise having to provide TF variables on each `terraform apply` invocation. All mentioned files are
located in the current folder and all commands must likewise be executed from this same folder.

The steps needed are:

- Create file `localhost.tfvars` from `localhost.tfvars.sample` and provide your personal values.
  (The format is the same as other terraform configuration files)
- Make sure that you're logged in with an account that has access permissions to the selected resource group.
  (Use `az login` from Azure CLI to switch account as needed)
- Temporarily rename `backend.tf` to `backend.tf.exclude`.
  (This is needed to avoid terraform from picking it up because the settings in the file are not overwritten by your
  `localhost.tfvars` file. Also Terraform currently doesn't support explicit exclusion. See [this issue](https://github.com/hashicorp/terraform/issues/2253))
- Execute `terraform init`
- Execute `terraform apply -var-file="localhost.tfvars"`
- Restore file name of `backend.tf`

Information about the settings:

- `environment` is used in resource name and must be lowercase
- `organisation` is used in resource name and must be lowercase

If you want to tear down all the resources again simply execute `terraform destroy -auto-approve`.

**Tip**: If you don't have provisioned any resources yet and encounter problems it might help to delete folder `.terraform` and file `.terraform.lock.hcl` and start over with `terraform init`.
