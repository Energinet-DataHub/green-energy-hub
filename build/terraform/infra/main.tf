resource "azurerm_key_vault" "key_vault" {
  name                        = var.keyvault_name
  location                    = var.location
  resource_group_name         = var.resource_group_name
  enabled_for_disk_encryption = true
  tenant_id                   = var.tenant_id
  soft_delete_enabled         = false
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = var.tenant_id
    object_id = var.object_id

    secret_permissions = [
      "set","get", "delete", "list"
    ]
  }
}

resource "azurerm_databricks_workspace" "databricks" {
  name                = var.databricks_name
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "standard"
}

resource "azurerm_eventhub_namespace" "eventhub_namespace" {
  name                = var.eventhub_namespace_name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Standard"
  capacity            = 1
}

resource "azurerm_eventhub" "input_eventhub" {
  name                = var.input_eventhub_name
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  resource_group_name = var.resource_group_name
  partition_count     = 32
  message_retention   = 1
}

resource "azurerm_eventhub" "valid_output_eventhub" {
  name                = var.valid_output_eventhub_name
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  resource_group_name = var.resource_group_name
  partition_count     = 32
  message_retention   = 1
}

resource "azurerm_eventhub" "invalid_output_eventhub" {
  name                = var.invalid_output_eventhub_name
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  resource_group_name = var.resource_group_name
  partition_count     = 32
  message_retention   = 1
}

resource "azurerm_eventhub_authorization_rule" "input_listen" {
  name                = "listen"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.input_eventhub.name
  resource_group_name = var.resource_group_name
  listen              = true
  send                = false
  manage              = false
}

resource "azurerm_key_vault_secret" "input_eventhub_listen_connection_string" {
  name         = "input-eventhub-listen-connection-string"
  value        = azurerm_eventhub_authorization_rule.input_listen.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_eventhub_authorization_rule" "input_send" {
  name                = "send"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.input_eventhub.name
  resource_group_name = var.resource_group_name
  listen              = false
  send                = true
  manage              = false
}

resource "azurerm_key_vault_secret" "input_eventhub_send_connection_string" {
  name         = "input-eventhub-send-connection-string"
  value        = azurerm_eventhub_authorization_rule.input_send.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_eventhub_authorization_rule" "valid_send" {
  name                = "send"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.valid_output_eventhub.name
  resource_group_name = var.resource_group_name
  listen              = false
  send                = true
  manage              = false
}

resource "azurerm_key_vault_secret" "valid_output_eventhub_send_connection_string" {
  name         = "valid-output-eventhub-send-connection-string"
  value        = azurerm_eventhub_authorization_rule.valid_send.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_eventhub_authorization_rule" "invalid_send" {
  name                = "send"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.invalid_output_eventhub.name
  resource_group_name = var.resource_group_name
  listen              = false
  send                = true
  manage              = false
}

resource "azurerm_key_vault_secret" "invalid_output_eventhub_send_connection_string" {
  name         = "invalid-output-eventhub-send-connection-string"
  value        = azurerm_eventhub_authorization_rule.invalid_send.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_storage_account" "storage" {
  name                     = var.storage_account_name
  resource_group_name      = var.resource_group_name
  location                 = var.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
  is_hns_enabled           = "true"  
}

resource "azurerm_key_vault_secret" "storage_account_key" {
  name         = "storage-account-key"
  value        = azurerm_storage_account.storage.primary_access_key
  key_vault_id = azurerm_key_vault.key_vault.id
}

resource "azurerm_storage_container" "streaming_stor_cont" {
  name                  = var.streaming_container_name
  storage_account_name  = azurerm_storage_account.storage.name
  container_access_type = "private"
}

resource "azurerm_storage_container" "aggregation_stor_cont" {
  name                  = var.aggregation_container_name
  storage_account_name  = azurerm_storage_account.storage.name
  container_access_type = "private"
}

resource "azurerm_application_insights" "appinsight" {
  name                  = var.appinsights_name
  resource_group_name   = var.resource_group_name
  location              = var.location
  application_type      = "other"
}

resource "azurerm_key_vault_secret" "appinsights_instrumentation_key" {
  name         = "appinsights-instrumentation-key"
  value        = azurerm_application_insights.appinsight.instrumentation_key
  key_vault_id = azurerm_key_vault.key_vault.id
}

