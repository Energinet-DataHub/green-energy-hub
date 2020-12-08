resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}
resource "null_resource" "dependency_setter" {
  depends_on = [azurerm_servicebus_queue_authorization_rule.main]
}

resource "azurerm_servicebus_queue_authorization_rule" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  namespace_name      = var.namespace_name
  queue_name          = var.queue_name
  resource_group_name = var.resource_group_name
  listen              = var.listen
  send                = var.send
  manage              = var.manage
}