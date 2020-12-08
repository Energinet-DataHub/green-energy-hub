terraform {
  required_version = ">= 0.13.5"

  required_providers {
    # It is recommended to pin to a given version of the Azure provider
    azurerm = "=2.36.0"
	  null = "~> 2.1"
  }
}

provider "azurerm" {
  features {
    key_vault {
      purge_soft_delete_on_destroy = true
    }
  }
}