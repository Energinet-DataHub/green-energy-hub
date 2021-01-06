output "id" {
  value       = azurerm_api_management.main.id
  description = "The ID of the API Management Service."
}

output "name" {
  value       = azurerm_api_management.main.name
  description = "The name of the API Management Service."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the App Service Plan component."
}