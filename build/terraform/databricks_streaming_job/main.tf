#Keeping secret for future secrets handling optimizaiton
#resource "databricks_secret_scope" "energinet_poc" {
#  name = "POC_Secret_Scope"
#}

#resource "databricks_secret" "eventhub_connection" {
#  key          = "eventhub_connection"
#  string_value = var.input_eh_listen_connection_string
#  scope        = databricks_secret_scope.energinet_poc.name
#}

data "azurerm_key_vault_secret" "appinsights_instrumentation_key" {
  name         = "appinsights-instrumentation-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "storage_account_key" {
  name         = "timeseries-storage-account-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "evhar_inboundqueue_receiver_connection_string" {
  name         = "evhar-inboundqueue-receiver-connection-string"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "valid_output_eventhub_send_connection_string" {
  name         = "temp-valid-sender-connection-string"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "invalid_output_eventhub_send_connection_string" {
  name         = "temp-invalid-sender-connection-string"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "cosmosdb_account_endpoint" {
  name         = "cosmosdb-account-endpoint"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "cosmosdb_account_primary_key" {
  name         = "cosmosdb-account-primary-key"
  key_vault_id = var.keyvault_id
}

module "streaming_job" {
  source                                         = "../job_modules/streaming_job"
  databricks_id                                  = var.databricks_id
  module_name                                    = "StreamingJob"
  storage_account_name                           = var.storage_account_name
  storage_account_key                            = data.azurerm_key_vault_secret.storage_account_key.value
  streaming_container_name                       = var.streaming_container_name
  input_eventhub_listen_connection_string        = data.azurerm_key_vault_secret.evhar_inboundqueue_receiver_connection_string.value
  valid_output_eventhub_send_connection_string   = data.azurerm_key_vault_secret.valid_output_eventhub_send_connection_string.value
  invalid_output_eventhub_send_connection_string = data.azurerm_key_vault_secret.invalid_output_eventhub_send_connection_string.value 
  appinsights_instrumentation_key                = data.azurerm_key_vault_secret.appinsights_instrumentation_key.value
  wheel_file                                     = var.wheel_file
  python_main_file                               = var.python_main_file
  cosmosdb-account-endpoint                      = data.azurerm_key_vault_secret.cosmosdb_account_endpoint.value
  cosmosdb-account-primary-key                   = data.azurerm_key_vault_secret.cosmosdb_account_primary_key.value
}
