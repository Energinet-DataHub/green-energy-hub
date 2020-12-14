module "azfun_asynchronousingestor" {
  source                                    = "../modules/function-app"
  name                                      = "azfun-asynchronousingestor-${var.environment}"
  resource_group_name                       = data.azurerm_resource_group.greenenergyhub.name
  location                                  = data.azurerm_resource_group.greenenergyhub.location
  storage_account_access_key                = module.azfun_asynchronousingestor_stor.primary_access_key
  storage_account_name                      = module.azfun_asynchronousingestor_stor.name
  app_service_plan_id                       = module.azfun_asynchronousingestor_plan.id
  application_insights_instrumentation_key  = module.appi_shared.instrumentation_key
  tags                                      = data.azurerm_resource_group.greenenergyhub.tags
  app_settings                              = {
    # Region: Default Values
    WEBSITE_ENABLE_SYNC_UPDATE_SITE       = true
    WEBSITE_RUN_FROM_PACKAGE              = 1
    WEBSITES_ENABLE_APP_SERVICE_STORAGE   = true
    FUNCTIONS_WORKER_RUNTIME              = "dotnet"
    # Endregion: Default Values
    KAFKA_SECURITY_PROTOCOL               = "SaslSsl"
    KAFKA_SASL_MECHANISM                  = "Plain"
    KAFKA_SSL_CA_LOCATION                 = "C:\\cacert\\cacert.pem"
    KAFKA_USERNAME                        = "$ConnectionString"
    KAFKA_MESSAGE_SEND_MAX_RETRIES        = 5
    KAFKA_MESSAGE_TIMEOUT_MS              = 1000
    REQUEST_QUEUE_URL                     = "${module.evhnm_requestqueue.name}.servicebus.windows.net:9093"
    REQUEST_QUEUE_CONNECTION_STRING       = module.evhar_requestqueue_listener.primary_connection_string
    VALIDATION_REPORTS_URL                = "${module.evhnm_validationreport.name}.servicebus.windows.net:9093"
    VALIDATION_REPORTS_CONNECTION_STRING  = module.evhar_validationreport_sender.primary_connection_string
    MARKET_DATA_URL                       = "${module.sbn_marketdata.name}.servicebus.windows.net:9093"
    MARKET_DATA_CONNECTION_STRING         = module.sbnar_marketdata_sender.primary_connection_string
  }
  dependencies                              = [
    module.appi_shared.dependent_on,
    module.azfun_asynchronousingestor_plan.dependent_on,
    module.azfun_asynchronousingestor_stor.dependent_on,
    module.evhnm_requestqueue.dependent_on,
    module.evhar_requestqueue_listener.dependent_on,
    module.evhnm_validationreport.dependent_on,
    module.evhar_validationreport_sender.dependent_on,
    module.sbn_marketdata.dependent_on,
    module.sbnar_marketdata_sender.dependent_on,
  ]
}

module "azfun_asynchronousingestor_plan" {
  source              = "../modules/app-service-plan"
  name                = "asp-asynchronousingestor-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  kind                = "FunctionApp"
  sku                 = {
    tier  = "Basic"
    size  = "B1"
  }
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "azfun_asynchronousingestor_stor" {
  source                    = "../modules/storage-account"
  name                      = "storasyncingest${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Cool"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}