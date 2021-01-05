module "appi_shared" {
  source              = "../modules/application-insights" 

  name                = "appi-shared-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "appinsights_instrumentation_key" {
  source       = "../modules/key-vault-secret"
  name         = "appinsights-instrumentation-key"
  value        = module.appi_shared.instrumentation_key
  key_vault_id = module.kv_shared.id
  dependencies   = [
    module.kv_shared.dependent_on, 
    module.appi_shared.dependent_on
  ]
}