resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_key_vault.main,
    azurerm_key_vault_access_policy.main,
  ]
}

data "azurerm_client_config" "main" {}

resource "azurerm_key_vault" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group_name
  tenant_id           = data.azurerm_client_config.main.tenant_id
  sku_name            = var.sku_name
  tags                = var.tags
}

resource "azurerm_key_vault_access_policy" "main" {
  depends_on              = [azurerm_key_vault.main]
  count                   = length(var.access_policy)
  key_vault_id            = azurerm_key_vault.main.id
  tenant_id               = var.access_policy[count.index].tenant_id
  object_id               = var.access_policy[count.index].object_id
  secret_permissions      = var.access_policy[count.index].secret_permissions
  key_permissions         = var.access_policy[count.index].key_permissions
  certificate_permissions = var.access_policy[count.index].certificate_permissions
  storage_permissions     = var.access_policy[count.index].storage_permissions
}