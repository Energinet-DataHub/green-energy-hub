resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_storage_container.main,
  ]
}

resource "azurerm_storage_container" "main" {
  depends_on            = [null_resource.dependency_getter]
  name                  = var.container_name
  storage_account_name  = var.storage_account_name
  container_access_type = var.container_access_type

}