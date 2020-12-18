module "kv_shared" {
  source                          = "../modules/key-vault"
  name                            = "kvshared${var.environment}"
  resource_group_name             = data.azurerm_resource_group.greenenergyhub.name
  location                        = data.azurerm_resource_group.greenenergyhub.location
  tags                            = data.azurerm_resource_group.greenenergyhub.tags
  enabled_for_template_deployment = true
  sku_name                        = "standard"
  
  access_policy = [
    {
      tenant_id               = var.current_tenant_id
      object_id               = var.current_spn_object_id
      secret_permissions      = ["set", "get", "list", "delete"]
      certificate_permissions = []
      key_permissions         = []
      storage_permissions     = []
    }
  ]
}