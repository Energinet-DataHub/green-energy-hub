module "stor_soaptojsonadapterstorage" {
  source                    = "../modules/storage-account"
  name                      = "storsoaptojsonids${lower(var.environment)}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Hot"
  account_tier              = "Standard"
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

module "ast_messagereferenceid" {
  source                = "../modules/table-storage"
  name                  = "astmsgrefid"
  storage_account_name  = module.stor_soaptojsonadapterstorage.name
  dependencies          = [module.stor_soaptojsonadapterstorage]
}

module "ast_transactionid" {
  source                = "../modules/table-storage"
  name                  = "asttransactionid"
  storage_account_name  = module.stor_soaptojsonadapterstorage.name
  dependencies          = [module.stor_soaptojsonadapterstorage]
}

module "ast_headerenergydocumentid" {
  source                = "../modules/table-storage"
  name                  = "asthedid"
  storage_account_name  = module.stor_soaptojsonadapterstorage.name
  dependencies          = [module.stor_soaptojsonadapterstorage]
}
