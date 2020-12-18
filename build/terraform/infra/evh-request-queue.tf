module "evhnm_requestqueue" {
  source                    = "../modules/event-hub-namespace"
  name                      = "evhnm-request-queue-${var.environment}"
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  location                  = data.azurerm_resource_group.greenenergyhub.location
  sku                       = "Standard"
  capacity                  = 1
  tags                      = data.azurerm_resource_group.greenenergyhub.tags
}

module "evh_requestqueue" {
  source                    = "../modules/event-hub"
  name                      = "evh-request-queue-${var.environment}"
  namespace_name            = module.evhnm_requestqueue.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  partition_count           = 32
  message_retention         = 1
  dependencies              = [module.evhnm_requestqueue]
}

module "evhar_requestqueue_sender" {
  source                    = "../modules/event-hub-auth-rule"
  name                      = "evhar-request-sender-${var.environment}"
  namespace_name            = module.evhnm_requestqueue.name
  eventhub_name             = module.evh_requestqueue.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  send                      = true
  dependencies              = [module.evh_requestqueue]
}

module "evhar_requestqueue_listener" {
  source                    = "../modules/event-hub-auth-rule"
  name                      = "evhar-request-listener-${var.environment}"
  namespace_name            = module.evhnm_requestqueue.name
  eventhub_name             = module.evh_requestqueue.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  listen                    = true
  dependencies              = [module.evh_requestqueue]
}