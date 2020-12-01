terraform {
  backend "azurerm" {
    container_name        = "tfstate"
    key                   = "terraform_infra.tfstate"
  }
}