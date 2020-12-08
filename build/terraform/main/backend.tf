terraform {
  backend "azurerm" {
    resource_group_name   = "rg-DataHub-D"
    storage_account_name  = "rgdatahubdtfstatedev"
    container_name        = "tfstate"
    key                   = "terraform_infra.tfstate"
  }
}