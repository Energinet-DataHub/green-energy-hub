module "stor_timeseries_data" {
  source                    = "../modules/storage-account"
  name                      = "timeseriesdata${var.environment}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  account_replication_type  = "LRS"
  access_tier               = "Hot"
  account_tier              = "Standard"
  is_hns_enabled            = true
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

module "timeseries_storage_account_key" {
  source       = "../modules/key-vault-secret"
  name         = "timeseries-storage-account-key"
  value        = module.stor_timeseries_data.primary_access_key
  key_vault_id = module.kv_shared.id
  dependencies   = [
    module.kv_shared.dependent_on,
    module.stor_timeseries_data.dependent_on
  ]
}

module "streaming_container" {
  source                = "../modules/storage-container"
  container_name        = var.streaming_container_name
  storage_account_name  = module.stor_timeseries_data.name
  container_access_type = "private"
  dependencies = [ module.stor_timeseries_data.dependent_on ]
}

module "aggregation_container" {
  source                = "../modules/storage-container"
  container_name        = var.aggregation_container_name
  storage_account_name  = module.stor_timeseries_data.name
  container_access_type = "private"
  dependencies = [ module.stor_timeseries_data.dependent_on ]
}

resource "azurerm_storage_blob" "master_data" {
  name                   = "master-data/master-data.csv"
  storage_account_name   = module.stor_timeseries_data.name
  storage_container_name = module.streaming_container.name
  type                   = "Block"
  source                 = "../../../samples/mock-data/master-data.csv"
}