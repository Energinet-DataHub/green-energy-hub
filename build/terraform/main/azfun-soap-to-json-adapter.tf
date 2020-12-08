module "azfun_soaptojsonadapter" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-soaptojsonadapter-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_soaptojsonadapter_stor.primary_access_key
  storage_account_name                      = module.azfun_soaptojsonadapter_stor.name
  app_service_plan_id                       = module.azfun_soaptojsonadapter_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE       = true,
    WEBSITE_RUN_FROM_PACKAGE              = 1,
    WEBSITES_ENABLE_APP_SERVICE_STORAGE   = true,
    FUNCTIONS_WORKER_RUNTIME              = "dotnet",
    SYNCHRONOUS_INGESTOR_BASE_URL         = "https://${module.azfun_synchronousingestor.default_hostname}"
  }
  connection_string                         = {
    SOAP_TO_JSON_STORAGE_ACCOUNT            = module.stor_soaptojsonadapterstorage.primary_connection_string
    VALIDATION_REPORTS_QUEUE                = module.evhar_validationreport_sender.primary_connection_string
  }
  dependencies                              = [module.azfun_soaptojsonadapter_plan.dependent_on, module.azfun_soaptojsonadapter_stor.dependent_on]
}

module "azfun_soaptojsonadapter_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-soaptojsonadapter-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_soaptojsonadapter_stor" {
  source                    = "../modules/storage-account"
  name                      = "storsoaptojson${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}