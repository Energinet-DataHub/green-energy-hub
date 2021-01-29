output "id" {
  value       = azurerm_data_factory_dataset_delimited_text.main.id
  description = "The ID of the delimited dataset."
}

output "name" {
  value       = azurerm_data_factory_dataset_delimited_text.main.name
  description = "The name of the delimited dataset."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the delimited dataset."
}
