terraform {
  backend "azurerm" {
    resource_group_name   = "rg-GreenEnergyHub_Sandbox-S"
    storage_account_name  = "enrgtfstateprod"
    container_name        = "tfstate"
    key                   = "terraform_infra.tfstate"
  }
}