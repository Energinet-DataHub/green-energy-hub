output "id" {
  value       = azurerm_eventhub_authorization_rule.main.id
  description = "The EventHub ID."
}

output "name" {
  value       = azurerm_eventhub_authorization_rule.main.name
  description = "The EventHub name."
}

output "dependent_on" {
  value = null_resource.dependency_setter.id
  description = "The EventHub dependencies."
}

output "primary_connection_string" {
  value       = azurerm_eventhub_authorization_rule.main.primary_connection_string
  description = "The Primary Connection String for the Event Hubs authorization Rule."
}