data "azurerm_key_vault_secret" "appinsights_instrumentation_key" {
  name         = "appinsights-instrumentation-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "storage_account_key" {
  name         = "timeseries-storage-account-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "receiver-evhar-inboundqueue-connection-string" {
  name         = var.receiver_eh_input_secret_name
  key_vault_id = var.keyvault_id
}

data "azurerm_cosmosdb_account" "cosdb" {
  name                = "postoffice-${var.environment}"
  resource_group_name = var.resource_group_name
}

module "streaming_job" {
  source                                         = "../job_modules/streaming_job"
  databricks_id                                  = var.databricks_id
  module_name                                    = "StreamingJob"
  storage_account_name                           = "timeseriesdata${var.environment}"
  storage_account_key                            = data.azurerm_key_vault_secret.storage_account_key.value
  streaming_container_name                       = var.streaming_container_name
  input_eventhub_listen_connection_string        = data.azurerm_key_vault_secret.receiver-evhar-inboundqueue-connection-string.value
  appinsights_instrumentation_key                = data.azurerm_key_vault_secret.appinsights_instrumentation_key.value
  wheel_file                                     = var.wheel_file
  python_main_file                               = var.python_main_file
  cosmosdb-account-endpoint                      = data.azurerm_cosmosdb_account.cosdb.endpoint
  cosmosdb-account-primary-key                   = data.azurerm_cosmosdb_account.cosdb.primary_master_key
  cosmosdb-database-name                         = "energinetDocsDB"
  cosmosdb-collection-name                       = var.cosmos_coll
}