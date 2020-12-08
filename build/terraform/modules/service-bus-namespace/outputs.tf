output "id" {
  value       = azurerm_servicebus_namespace.main.id
  description = "The ServiceBus Namespace ID."
}

output "name" {
  value       = azurerm_servicebus_namespace.main.name
  description = "The ServiceBus Namespace name."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The ServiceBus Namespace dependencies."
}