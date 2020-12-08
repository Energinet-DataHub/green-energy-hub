resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}
resource "null_resource" "dependency_setter" {
  depends_on = [azurerm_key_vault_secret.secret]
}

resource "azurerm_key_vault_secret" "secret" {
  depends_on    = [null_resource.dependency_getter]
  name          = var.name
  value         = var.value
  key_vault_id  = var.key_vault_id
  tags          = var.tags
}