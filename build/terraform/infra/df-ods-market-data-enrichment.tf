module "ods-pipline-market-data-enrichment" {
  source                      = "../modules/data-factory-pipeline"
  name                        = "ods-market-data-enrichment-pipeline"
  resource_group_name         = data.azurerm_resource_group.greenenergyhub.name
  data_factory_name           = module.data-factory.name
  activities_json             = null
}

module "blob-linked-service" {
  source                      = "../modules/data-factory-linked-service-blob"
  name                        = "blob-linked-service"
  resource_group_name         = data.azurerm_resource_group.greenenergyhub.name
  data_factory_name           = module.data-factory.name
  connection_string           = module.stor_timeseries_data.primary_connection_string
}

module "market-data-sql-database-linked-service" {
  source                      = "../modules/data-factory-linked-service-sql-server"
  name                        = "sql-market-data-linked-service"
  resource_group_name         = data.azurerm_resource_group.greenenergyhub.name
  data_factory_name           = module.data-factory.name
  connection_string           = "server=tcp:${module.sqlsrv_marketdata.fully_qualified_domain_name}; database=${module.sqldb_marketdata.name}; user id=${local.sqlServerAdminName};password=${random_password.sqlsrv_admin_password.result}"
}

module "sink-market-data-enrichment-data-set" {
  source                      = "../modules/data-factory-dataset-delimited-text"
  name                        = "sink-market-data-enrichment-csv-data-set"
  resource_group_name         = data.azurerm_resource_group.greenenergyhub.name
  data_factory_name           = module.data-factory.name
  linked_service_name         = module.blob-linked-service.name
  azure_blob_storage_location = {
    container   = var.streaming_container_name
    path        = "market-data"
    filename    = "market-data-enrichment.csv"
  }
  column_delimiter            = ","
  row_delimiter               = "\r\n"
  encoding                    = "UTF-8"
  quote_character             = "\""
  escape_character            = "\\"
  first_row_as_header         = true
  null_value                  = "NULL"
}

module "source-market-data-sql-data-set" {
  source                      = "../modules/data-factory-dataset-sql-table"
  name                        = "source-market-data-sql-data-set"
  resource_group_name         = data.azurerm_resource_group.greenenergyhub.name
  data_factory_name           = module.data-factory.name
  linked_service_name         = module.market-data-sql-database-linked-service.name
  table_name                  = "dbo.ODSMasterDataEnrichment"
}