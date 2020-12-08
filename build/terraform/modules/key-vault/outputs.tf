output "id" {
  value       = azurerm_key_vault.main.id
  description = "The ID of the Key Vault."
}

output "name" {
  value       = azurerm_key_vault.main.name
  description = "The name of the Key Vault."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Key Vault."
}

output "vault_uri" {
  value       = azurerm_key_vault.main.vault_uri
  description = "The URI of the Key Vault, used for performing operations on keys and secrets."
}