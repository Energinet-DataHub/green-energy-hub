output "id" {
  value       = azurerm_data_factory.main.id
  description = "The ID of the Data Factory."
}

output "name" {
  value       = azurerm_data_factory.main.name
  description = "The name of the Data Factory."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Data Factory."
}
