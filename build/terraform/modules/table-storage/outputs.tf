output "id" {
  value       = azurerm_storage_table.main.id
  description = "The id of the resource created."
}

output "name" {
  value       = azurerm_storage_table.main.name
  description = "The name of the resource created."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the resource created."
}