output "databricks_id" {
  value = azurerm_databricks_workspace.databricks.id
}

output "databricks_workspace_id" {
  value = azurerm_databricks_workspace.databricks.workspace_id
}

output "databricks_workspace_url" {
  value = azurerm_databricks_workspace.databricks.workspace_url
}

output "input_eh_listen_connection_string" {
  value     = azurerm_eventhub_authorization_rule.input_listen.primary_connection_string
  sensitive = true
}

output "valid_output_eh_send_connection_string" {
  value     = azurerm_eventhub_authorization_rule.valid_send.primary_connection_string
  sensitive = true
}

output "invalid_output_eh_send_connection_string" {
  value     = azurerm_eventhub_authorization_rule.invalid_send.primary_connection_string
  sensitive = true
}

output "storage_key" {
  value     = azurerm_storage_account.storage.primary_access_key
  sensitive = true
}

output "storage_name" {
  value = var.storage_account_name
}

output "streaming_container_name" {
  value = azurerm_storage_container.streaming_stor_cont.name
}

output "telemetry_instrumentation_key" {
  value = azurerm_application_insights.appinsight.instrumentation_key
  sensitive = true
}

