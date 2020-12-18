module "azfun_jsonschemavalidator" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-jsonschemavalidator-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_jsonschemavalidator_stor.primary_access_key
  storage_account_name                      = module.azfun_jsonschemavalidator_stor.name
  app_service_plan_id                       = module.azfun_jsonschemavalidator_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE     = true,
    WEBSITE_RUN_FROM_PACKAGE            = 1,
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = true,
    FUNCTIONS_WORKER_RUNTIME            = "dotnet",
    # Endregion: Default Values
  }
  dependencies                              = [
    module.appi_shared.dependent_on,
    module.azfun_jsonschemavalidator_plan.dependent_on, 
    module.azfun_jsonschemavalidator_stor.dependent_on,
  ]
}

module "azfun_jsonschemavalidator_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-jsonschemavalidator-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_jsonschemavalidator_stor" {
  source                    = "../modules/storage-account"
  name                      = "storjsonvalidator${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}