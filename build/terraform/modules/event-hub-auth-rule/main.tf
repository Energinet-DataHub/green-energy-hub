resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
  command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_eventhub_authorization_rule.main,
  ]
}

resource "azurerm_eventhub_authorization_rule" "main" {
  depends_on          = [null_resource.dependency_getter]
  namespace_name      = var.namespace_name
  eventhub_name       = var.eventhub_name
  resource_group_name = var.resource_group_name
  name                = var.name
  listen              = var.listen
  send                = var.send
  manage              = var.manage
}
