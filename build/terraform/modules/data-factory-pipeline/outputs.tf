output "id" {
  value       = azurerm_data_factory_pipeline.main.id
  description = "The ID of the Data Factory Pipeline."
}

output "name" {
  value       = azurerm_data_factory_pipeline.main.name
  description = "The name of the Data Factory Pipeline."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Data Factory Pipeline."
}
