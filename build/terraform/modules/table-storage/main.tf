resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_storage_table.main,
  ]
}

resource "azurerm_storage_table" "main" {
  depends_on           = [null_resource.dependency_getter]
  name                 = var.name
  storage_account_name = var.storage_account_name
}