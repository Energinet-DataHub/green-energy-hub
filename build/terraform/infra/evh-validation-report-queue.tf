module "evhnm_validationreport" {
  source                    = "../modules/event-hub-namespace"
  name                      = "evhnm-validation-reports-${var.environment}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  sku                       = "Standard"
  capacity                  = 1
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

module "evh_validationreport" {
  source                    = "../modules/event-hub"
  name                      = "evh-validation-reports-${var.environment}"
  namespace_name            = module.evhnm_validationreport.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  partition_count           = 32
  message_retention         = 1
  dependencies              = [module.evhnm_validationreport]
}

module "evhar_validationreport_sender" {
  source                    = "../modules/event-hub-auth-rule"
  name                      = "evhar-request-sender-${var.environment}"
  namespace_name            = module.evhnm_validationreport.name
  eventhub_name             = module.evh_validationreport.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  send                      = true
  dependencies              = [module.evh_validationreport]
}

module "evhar_validationreport_receiver" {
  source                    = "../modules/event-hub-auth-rule"
  name                      = "evhar-request-receivers-${var.environment}"
  namespace_name            = module.evhnm_validationreport.name
  eventhub_name             = module.evh_validationreport.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  listen                    = true
  dependencies              = [module.evh_validationreport]
}