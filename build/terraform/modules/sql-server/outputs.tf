output "id" {
  value       = azurerm_sql_server.main.id
  description = "The Microsoft SQL Server ID."
}

output "name" {
  value       = azurerm_sql_server.main.name
  description = "The Microsoft SQL Server name."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The Microsoft SQL Server dependencies."
}

output "fully_qualified_domain_name" {
  value       = azurerm_sql_server.main.fully_qualified_domain_name
  description = "The fully qualified domain name of the Azure SQL Server (e.g. myServerName.database.windows.net)"
}