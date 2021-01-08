module "azfun_timertrigger" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-timertrigger-${var.organisation}-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_timertrigger_stor.primary_access_key
  storage_account_name                      = module.azfun_timertrigger_stor.name
  app_service_plan_id                       = module.azfun_timertrigger_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE          = true
    WEBSITE_RUN_FROM_PACKAGE                 = 1
    WEBSITES_ENABLE_APP_SERVICE_STORAGE      = true
    FUNCTIONS_WORKER_RUNTIME                 = "dotnet"
    # Endregion: Default Values
    MARKET_DATA_DATABASE                     = "Server=${module.sqlsrv_marketdata.fully_qualified_domain_name};Database=${module.sqldb_marketdata.name};User Id=${local.sqlServerAdminName};Password=${random_password.sqlsrv_admin_password.result};"
    MARKET_DATA_EVENT_QUEUE_ENDPOINT         = module.evt_marketdataeventqueue.endpoint
    MARKET_DATA_EVENT_QUEUE_CONNECTIONSTRING = module.evt_marketdataeventqueue.primary_access_key
  }
  dependencies                              = [
    module.appi_shared.dependent_on,
    module.azfun_timertrigger_plan.dependent_on,
    module.azfun_timertrigger_stor.dependent_on,
    module.sqlsrv_marketdata.dependent_on,
    module.sqldb_marketdata.dependent_on,
    module.evt_marketdataeventqueue.dependent_on,
  ]
}

module "azfun_timertrigger_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-timertrigger-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_timertrigger_stor" {
  source                    = "../modules/storage-account"
  name                      = "stor${random_string.timertrigger.result}${var.organisation}${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

# Since all functions need a storage connected we just generate a random name
resource "random_string" "timertrigger" {
  length  = 5
  special = false
  upper   = false
}