resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
  command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_eventgrid_topic.main,
  ]
}

resource "azurerm_eventgrid_topic" "main" {
  depends_on                   = [null_resource.dependency_getter]
  name                         = var.name
  resource_group_name          = var.resource_group_name
  location                     = var.location
  input_schema                 = var.input_schema
  tags                         = var.tags
}