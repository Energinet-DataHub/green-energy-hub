output "id" {
  value       = azurerm_app_service_plan.main.id
  description = "The ID of the App Service Plan component."
}

output "name" {
  value       = azurerm_app_service_plan.main.name
  description = "The name of the App Service Plan component."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the App Service Plan component."
}