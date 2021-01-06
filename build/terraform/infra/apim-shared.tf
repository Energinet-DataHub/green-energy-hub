module "apim_shared" {
  source              = "../modules/api-management" 

  name                = "apim-shared-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  publisher_name      = "Energinet"
  publisher_email     = "mea@energinet.dk"
  sku_name            = "Basic_1"
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}