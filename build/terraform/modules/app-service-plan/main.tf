# If you modify the module, remember to bump version (For easy tracking of old module versions deployed)

resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_app_service_plan.main,
  ]
}

resource "azurerm_app_service_plan" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  kind                = var.kind
  tags                = var.tags

  sku {
    size = var.sku.size
    tier = var.sku.tier
  }
}