module "stor_validationreportsstorage" {
  source                    = "../modules/storage-account"
  name                      = "storvalreports${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Hot"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

module "ast_validationreports" {
  source                = "../modules/table-storage"
  name                  = "astvalreports"
  storage_account_name  = module.stor_validationreportsstorage.name
  dependencies          = [module.stor_validationreportsstorage]
}