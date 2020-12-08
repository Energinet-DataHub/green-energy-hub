module "azfun_soapschemavalidator" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-soapschemavalidator-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_soapschemavalidator_stor.primary_access_key
  storage_account_name                      = module.azfun_soapschemavalidator_stor.name
  app_service_plan_id                       = module.azfun_soapschemavalidator_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE       = true,
    WEBSITE_RUN_FROM_PACKAGE              = 1,
    WEBSITES_ENABLE_APP_SERVICE_STORAGE   = true,
    FUNCTIONS_WORKER_RUNTIME              = "dotnet",
  }
  connection_string                         = {
    VALIDATION_REPORTS_QUEUE                = module.evhar_validationreport_sender.primary_connection_string
  }
  dependencies                              = [module.azfun_soapschemavalidator_plan.dependent_on, module.azfun_soapschemavalidator_stor.dependent_on]
}

module "azfun_soapschemavalidator_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-soapschemavalidator-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_soapschemavalidator_stor" {
  source                    = "../modules/storage-account"
  name                      = "storsoapvalidator${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}