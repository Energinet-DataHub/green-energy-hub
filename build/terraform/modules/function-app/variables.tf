variable name {
  type        = string
  description = "(Required) Specifies the name of the Function App. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the Function App."
}

variable location {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
}

variable app_service_plan_id {
  type        = string
  description = "(Required) The ID of the App Service Plan within which to create this Function App." 
}

variable application_insights_instrumentation_key {
  type        = string
  description = "The application insights instrumentation key for which data is to be logged into"
}

variable app_settings {
  type        = map(string)
  description = "(Optional) A map of key-value pairs for App Settings and custom values." 
  default     = {}
}

variable connection_string {
  type        = map(string)
  description = "(Optional) An connection_string block as defined below."
  default     = {}
}

variable always_on {
  type        = bool
  description = "(Optional) Should the Function App be loaded at all times? Defaults to false"
  default     = false
}

variable storage_account_access_key {
  type        = string
  description = "(Required) The backend storage account name which will be used by this Function App (such as the dashboard, logs)."
}

variable storage_account_name {
  type        = string
  description = "(Required) The access key which will be used to access the backend storage account for the Function App."
}

variable tags {
  type        = any
  description = "(Optional) A mapping of tags to assign to the resource."
  default     = {}
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}