output "id" {
  value       = azurerm_eventhub_namespace.main.id
  description = "The EventHub Namespace ID."
}

output "name" {
  value       = azurerm_eventhub_namespace.main.name
  description = "The EventHub Namespace name."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The EventHub Namespace dependencies."
}