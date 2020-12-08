output "id" {
  value       = azurerm_servicebus_queue.main.id
  description = "The ServiceBus Queue ID."
}

output "name" {
  value       = azurerm_servicebus_queue.main.name
  description = "The ServiceBus Queue ID."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The ServiceBus Queue dependencies."
}