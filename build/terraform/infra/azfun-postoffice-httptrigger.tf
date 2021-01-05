module "azfun_postoffice" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-postoffice-${var.organisation}-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_postoffice_stor.primary_access_key
  storage_account_name                      = module.azfun_postoffice_stor.name
  app_service_plan_id                       = module.azfun_postoffice_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    CosmosDbEndpoint  = azurerm_cosmosdb_account.acc.endpoint,
    CosmosDbKey       = azurerm_cosmosdb_account.acc.primary_key
  }
  dependencies                              = [
    module.azfun_postoffice_plan.dependent_on,
    module.azfun_postoffice_stor.dependent_on,
  ]
}

module "azfun_postoffice_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-postoffice-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
}

module "azfun_postoffice_stor" {
  source                    = "../modules/storage-account"
  name                      = "stor${random_string.postoffice.result}${var.organisation}${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
}

# Since all functions need a storage connected we just generate a random name
resource "random_string" "postoffice" {
  length  = 5
  special = false
  upper   = false
}