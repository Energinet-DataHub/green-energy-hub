output "id" {
  value       = azurerm_application_insights.main.id
  description = "The ID of the Application Insights component."
}

output "name" {
  value       = azurerm_application_insights.main.name
  description = "The name of the Application Insights component."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Application Insights component."
}

output "instrumentation_key" {
  value       = azurerm_application_insights.main.instrumentation_key
  description = "The Instrumentation Key for this Application Insights component."
}