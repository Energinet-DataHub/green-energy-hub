resource "azurerm_databricks_workspace" "databricks" {
  name                = "dbw-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  sku                 = "standard"
}



