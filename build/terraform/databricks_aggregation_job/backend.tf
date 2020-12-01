terraform {
  backend "azurerm" {
    container_name        = "tfstate"
    key                   = "terraform_databricks_aggregation_job.tfstate"
  }
}