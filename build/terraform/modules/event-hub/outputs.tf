output "id" {
  value       = azurerm_eventhub.main.id
  description = "The ID of the EventHub."
}

output "name" {
  value       = azurerm_eventhub.main.name
  description = "The name of the EventHub."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the EventHub."
}