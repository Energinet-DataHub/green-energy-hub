module "sbn_marketdata" {
  source              = "../modules/service-bus-namespace"
  name                = "sbn-marketdata-${var.organisation}-${var.environment}"
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  location            = data.azurerm_resource_group.greenenergyhub.location
  sku                 = "basic"
  tags                = data.azurerm_resource_group.greenenergyhub.tags
}

module "sbq_marketdata" {
  source              = "../modules/service-bus-queue"
  name                = "sbq-marketdata"
  namespace_name      = module.sbn_marketdata.name
  resource_group_name = data.azurerm_resource_group.greenenergyhub.name
  dependencies        = [module.sbn_marketdata]
}

module "sbnar_marketdata_listener" {
  source                    = "../modules/service-bus-queue-auth-rule"
  name                      = "sbnar-marketdata-listener"
  namespace_name            = module.sbn_marketdata.name
  queue_name                = module.sbq_marketdata.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  listen                    = true
  dependencies              = [module.sbq_marketdata]
}

module "sbnar_marketdata_sender" {
  source                    = "../modules/service-bus-queue-auth-rule"
  name                      = "sbnar-marketdata-sender"
  namespace_name            = module.sbn_marketdata.name
  queue_name                = module.sbq_marketdata.name
  resource_group_name       = data.azurerm_resource_group.greenenergyhub.name
  send                      = true
  dependencies              = [module.sbq_marketdata]
}