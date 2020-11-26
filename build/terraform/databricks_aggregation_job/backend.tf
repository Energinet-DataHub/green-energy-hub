terraform {
  backend "azurerm" {
    resource_group_name   = "rg-GreenEnergyHub_Sandbox-S"
    storage_account_name  = "energinettfstate"
    container_name        = "tfstate"
    key                   = "terraform_databricks_aggregation_job.tfstate"
  }
}