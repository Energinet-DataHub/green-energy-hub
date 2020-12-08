variable name {
  type        = string
  description = "(Required) Specifies the name of the Authorization Rule. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the Resource Group in which the ServiceBus Namespace exists. Changing this forces a new resource to be created."
}

variable namespace_name {
  type        = string
  description = "(Required) Specifies the name of the ServiceBus Namespace in which the Queue exists. Changing this forces a new resource to be created."
}

variable queue_name {
  type        = string
  description = "(Required) Specifies the name of the ServiceBus Queue. Changing this forces a new resource to be created."
}

variable listen {
  type        = bool
  description = "(Optional) Grants listen access to this this Authorization Rule. Defaults to false."
  default     = false
}

variable send {
  type        = bool
  description = "(Optional) Grants send access to this this Authorization Rule. Defaults to false."
  default     = false
}

variable manage {
  type        = bool
  description = "(Optional) Grants manage access to this this Authorization Rule. When this property is true - both listen and send must be too. Defaults to false."
  default     = false
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}
