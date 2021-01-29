output "id" {
  value       = azurerm_data_factory_linked_service_azure_blob_storage.main.id
  description = "The ID of the Blob storage linked service."
}

output "name" {
  value       = azurerm_data_factory_linked_service_azure_blob_storage.main.name
  description = "The name of the Blob storage linked service."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Blob storage linked service."
}
