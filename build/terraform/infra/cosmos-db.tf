resource "azurerm_cosmosdb_account" "acc" {
  name                = "postoffice-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  # To enable global failover change to true and uncomment second geo_location
  enable_automatic_failover = false

  consistency_policy {
    consistency_level = "Session"
  }
  
  geo_location {
    location          = data.azurerm_resource_group.greenenergyhub.location
    failover_priority = 0
  }
  # geo_location {
  #   location          = "<secondary location>"
  #   failover_priority = 1
  # }
}

resource "azurerm_cosmosdb_sql_database" "db" {
  name                = "energinetDocsDB"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  account_name        = azurerm_cosmosdb_account.acc.name
}

resource "azurerm_cosmosdb_sql_container" "coll" {
  name                = "energinetDocsContainer"
  resource_group_name = var.resource_group_name
  account_name        = azurerm_cosmosdb_account.acc.name
  database_name       = azurerm_cosmosdb_sql_database.db.name
  partition_key_path  = "/Vendor"
}

module "cosmosdb_account_endpoint" {
  source       = "../modules/key-vault-secret"
  name         = "cosmosdb-account-endpoint"
  value        = azurerm_cosmosdb_account.acc.endpoint
  key_vault_id = module.kv_shared.id
  dependencies = [
      module.kv_shared.dependent_on 
  ]
}

module "cosmosdb-account-primary-key" {
  source       = "../modules/key-vault-secret"
  name         = "cosmosdb-account-primary-key"
  value        = azurerm_cosmosdb_account.acc.primary_key
  key_vault_id = module.kv_shared.id
  dependencies = [
      module.kv_shared.dependent_on 
  ]
}
