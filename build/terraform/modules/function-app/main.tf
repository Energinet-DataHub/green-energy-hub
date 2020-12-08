resource "null_resource" "dependency_getter" {
  provisioner "local-exec" {
    command = "echo ${length(var.dependencies)}"
  }
}

resource "null_resource" "dependency_setter" {
  depends_on = [azurerm_function_app.main]
}

resource "azurerm_function_app" "main" {
  depends_on                  = [null_resource.dependency_getter]
  name                        = var.name
  location                    = var.location
  resource_group_name         = var.resource_group_name
  app_service_plan_id         = var.app_service_plan_id
  storage_account_name        = var.storage_account_name
  storage_account_access_key  = var.storage_account_access_key
  version                     = "~3"
  tags                        = var.tags
  https_only                  = true
  app_settings                = merge({
    APPINSIGHTS_INSTRUMENTATIONKEY = var.application_insights_instrumentation_key
  },var.app_settings)

  dynamic "connection_string" {
    for_each = var.connection_string
    content {
      name    = connection_string.key
      value   = connection_string.value
      type    = "Custom"
    }
  }
  
  identity {
    type = "SystemAssigned"
  }
  
  site_config {
    always_on                   = var.always_on
    cors {
      allowed_origins = ["*"]
    }
  }

  lifecycle {
    ignore_changes = [
      source_control
    ]
  }
}