output "id" {
  value       = azurerm_data_factory_dataset_sql_server_table.main.id
  description = "The ID of the SQL table dataset."
}

output "name" {
  value       = azurerm_data_factory_dataset_sql_server_table.main.name
  description = "The name of the SQL table dataset."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the SQL table dataset."
}
