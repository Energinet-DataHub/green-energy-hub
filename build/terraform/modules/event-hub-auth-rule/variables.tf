variable name {
  type        = string
  description = "(Required) Specifies the name of the EventHub Authorization Rule resource. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which the EventHub's parent Namespace exists. Changing this forces a new resource to be created."
}

variable namespace_name {
  type        = string
  description = "(Required) Specifies the name of the grandparent EventHub Namespace. Changing this forces a new resource to be created."
}

variable eventhub_name {
  type        = string
  description = "(Required) Specifies the name of the EventHub. Changing this forces a new resource to be created."
}

variable listen {
  type        = bool
  description = "(Optional) Does this Authorization Rule have permissions to Listen to the Event Hub? Defaults to false."
  default     = false
}

variable send {
  type        = bool
  description = "(Optional) Does this Authorization Rule have permissions to Send to the Event Hub? Defaults to false."
  default     = false
}

variable manage {
  type        = bool
  description = "(Optional) Does this Authorization Rule have permissions to Manage to the Event Hub? When this property is true - both listen and send must be too. Defaults to false"
  default     = false
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}
