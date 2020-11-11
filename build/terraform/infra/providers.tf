#Set the terraform required version

terraform {
  required_version = ">= 0.12.6"

  required_providers {
    # It is recommended to pin to a given version of the Azure provider
    azurerm = "=2.31.1"
	  null = "~> 2.1"
  }
}

provider "azurerm" {
  # It is recommended to pin to a given version of the Provider
  features {}
}
