output "id" {
  value       = azurerm_key_vault_secret.secret.id
  description = "The Key Vault Secret ID."
}

output "name" {
  value       = azurerm_key_vault_secret.secret.name
  description = "The Key Vault Secret name."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The Key Vault Secret dependencies."
}

output "version" {
  value       = azurerm_key_vault_secret.secret.version
  description = "The current version of the Key Vault Secret."
}