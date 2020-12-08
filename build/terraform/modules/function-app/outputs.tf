output "id" {
  value       = azurerm_function_app.main.id
  description = "The ID of the Function App."
}

output "name" {
  value       = azurerm_function_app.main.name
  description = "The name of the Function App."
}

output "dependent_on" {
  value       = null_resource.dependency_setter.id
  description = "The dependencies of the Function App."
}

output "default_hostname" {
  value       = azurerm_function_app.main.default_hostname
  description = "The default hostname associated with the Function App - such as mysite.azurewebsites.net"
}

output "outbound_ip_addresses" {
  value       = azurerm_function_app.main.outbound_ip_addresses
  description = "A comma separated list of outbound IP addresses - such as 52.23.25.3,52.143.43.12"
}

output "possible_outbound_ip_addresses" {
  value       = azurerm_function_app.main.possible_outbound_ip_addresses
  description = " comma separated list of outbound IP addresses - such as 52.23.25.3,52.143.43.12,52.143.43.17 - not all of which are necessarily in use. Superset of outbound_ip_addresses"
}

output "identity" {
  value       = azurerm_function_app.main.identity
  description = "An identity block as defined below, which contains the Managed Service Identity information for this App Service."
}

output "site_credential" {
  value       = azurerm_function_app.main.site_credential
  description = "A site_credential block as defined below, which contains the site-level credentials used to publish to this App Service."
}

output "kind" {
  value       = azurerm_function_app.main.kind
  description = "The Function App kind - such as functionapp,linux,container"
}

# The identity block exports the following:
# principal_id - The Principal ID for the Service Principal associated with the Managed Service Identity of this App Service.
# tenant_id - The Tenant ID for the Service Principal associated with the Managed Service Identity of this App Service.

# The site_credential block exports the following:
# username - The username which can be used to publish to this App Service
# password - The password associated with the username, which can be used to publish to this App Service.