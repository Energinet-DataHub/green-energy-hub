output "id" {
  value       = azurerm_data_factory_linked_service_sql_server.main.id
  description = "The ID of the SQL server linked service."
}

output "name" {
  value       = azurerm_data_factory_linked_service_sql_server.main.name
  description = "The name of the SQL server linked service."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the SQL server linked service."
}
