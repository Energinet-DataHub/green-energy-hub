module "evt_marketdataeventqueue" {
    source              = "../modules/event-grid-topic"
    name                = "evt-marketdataeventqueue-${var.organisation}-${var.environment}"
    location = data.azurerm_resource_group.greenenergyhub.location
    resource_group_name = data.azurerm_resource_group.greenenergyhub.name
}