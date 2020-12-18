resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [
    azurerm_storage_account.main,
  ]
}

resource "azurerm_storage_account" "main" {
  depends_on                        = [null_resource.dependency_getter]
  name                              = var.name
  resource_group_name               = var.resource_group_name 
  location                          = var.location 
  account_tier                      = "Standard"
  account_replication_type          = "LRS"
  is_hns_enabled                    = var.is_hns_enabled
  tags                              = var.tags
}