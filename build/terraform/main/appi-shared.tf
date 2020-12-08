module "appi_shared" {
  source              = "../modules/application-insights" 

  name                = "appi-shared-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}