module "azfun_postoffice_httptrigger" {
  source                                    = "../modules/function-app"
  name                                      = "postoffice-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_postoffice_httptrigger_stor.primary_access_key
  storage_account_name                      = module.azfun_postoffice_httptrigger_stor.name
  app_service_plan_id                       = module.azfun_postoffice_httptrigger_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    CosmosDbEndpoint  = azurerm_cosmosdb_account.acc.endpoint,
    CosmosDbKey       = azurerm_cosmosdb_account.acc.primary_key
  }
  dependencies                              = [
    module.azfun_postoffice_httptrigger_plan.dependent_on,
    module.azfun_postoffice_httptrigger_stor.dependent_on,
  ]
}

module "azfun_postoffice_httptrigger_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-postoffice-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
}

module "azfun_postoffice_httptrigger_stor" {
  source                    = "../modules/storage-account"
  name                      = "storpostoffice${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
}