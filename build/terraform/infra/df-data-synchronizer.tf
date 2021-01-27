module "data-factory" {
  source                    = "../modules/data-factory"
  name                      = "df-data-synchronizer-${var.organisation}-${var.environment}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}
