output "id" {
  value       = azurerm_eventgrid_topic.main.id
  description = "The EventGrid Topic ID."
}

output endpoint {
  value = azurerm_eventgrid_topic.main.endpoint
  description = " The Endpoint associated with the EventGrid Topic."
}

output "primary_access_key" {
  value       = azurerm_eventgrid_topic.main.primary_access_key
  description = "The Primary Shared Access Key associated with the EventGrid Topic."
}

output "secondary_access_key" {
  value       = azurerm_eventgrid_topic.main.secondary_access_key
  description = "The Secondary Shared Access Key associated with the EventGrid Topic."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Event Grid Topic component."
}