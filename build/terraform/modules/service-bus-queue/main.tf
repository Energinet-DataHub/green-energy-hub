resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}
resource "null_resource" "dependency_setter" {
  depends_on = [azurerm_servicebus_queue.main]
}

resource "azurerm_servicebus_queue" "main" {
  depends_on          = [null_resource.dependency_getter]
  name                = var.name
  resource_group_name = var.resource_group_name
  namespace_name      = var.namespace_name

  enable_partitioning = true
}