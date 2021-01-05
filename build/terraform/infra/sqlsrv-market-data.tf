locals {
  sqlServerAdminName = "gehdbadmin"
}

module "sqlsrv_marketdata" {
  source                        = "../modules/sql-server"
  name                          = "sqlsrv-marketdata-${var.organisation}-${var.environment}"
  resource_group_name           = data.azurerm_resource_group.greenenergyhub.name
  location                      = data.azurerm_resource_group.greenenergyhub.location
  administrator_login           = local.sqlServerAdminName
  administrator_login_password  = random_password.sqlsrv_admin_password.result
  tags                          = data.azurerm_resource_group.greenenergyhub.tags
}

module "sqldb_marketdata" {
  source              = "../modules/sql-database"
  name                = "sqldb-marketdata"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  tags                = data.azurerm_resource_group.greenenergyhub.tags
  server_name         = module.sqlsrv_marketdata.name
  dependencies        = [module.sqlsrv_marketdata.dependent_on]
}

module "sqlsrv_admin_username" {
  source        = "../modules/key-vault-secret"
  name          = "SQLSERVER--ADMIN--USER"
  value         = local.sqlServerAdminName
  key_vault_id  = module.kv_shared.id
  tags          = data.azurerm_resource_group.greenenergyhub.tags
  dependencies  = [module.kv_shared.dependent_on]
}

module "sqlsrv_admin_password" {
  source        = "../modules/key-vault-secret"
  name          = "SQLSERVER--ADMIN--PASSWORD"
  value         = random_password.sqlsrv_admin_password.result
  key_vault_id  = module.kv_shared.id
  tags          = data.azurerm_resource_group.greenenergyhub.tags
  dependencies  = [module.kv_shared.dependent_on]
}

resource "random_password" "sqlsrv_admin_password" {
  length = 16
  special = true
  override_special = "_%@"
}