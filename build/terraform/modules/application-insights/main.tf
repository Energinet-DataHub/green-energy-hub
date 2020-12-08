# If you modify the module, remember to bump version (For easy tracking of old module versions deployed)

resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [azurerm_application_insights.main]
}

resource "azurerm_application_insights" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  application_type    = "web"
  tags                = var.tags
}