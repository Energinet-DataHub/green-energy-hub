module "azfun_businessworkflow" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-businessworkflow-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_businessworkflow_stor.primary_access_key
  storage_account_name                      = module.azfun_businessworkflow_stor.name
  app_service_plan_id                       = module.azfun_businessworkflow_plan.id
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
    MASTERDATA_INBOUND_QUEUE                = module.sbnar_marketdata_businesswflow.primary_connection_string
    MASTERDATA_DATABASE                     = "Server=${module.sqlsrv_masterdata.fully_qualified_domain_name};Database=${module.sqldb_masterdata.name};User Id=${local.sqlServerAdminName};Password=${random_password.sqlsrv_admin_password.result};"
  }
  dependencies                              = [
    module.azfun_businessworkflow_plan.dependent_on,
    module.azfun_businessworkflow_stor.dependent_on,
    module.sqlsrv_masterdata.dependent_on,
    module.sbnar_marketdata_businesswflow.dependent_on,
    module.evhar_validationreport_sender.dependent_on
  ]
}

module "azfun_businessworkflow_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-businessworkflow-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_businessworkflow_stor" {
  source                    = "../modules/storage-account"
  name                      = "storbizwrkflow${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}