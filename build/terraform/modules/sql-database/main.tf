resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_sql_database.main,
  ]
}

resource "azurerm_sql_database" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  server_name         = var.server_name
  tags                = var.tags
}