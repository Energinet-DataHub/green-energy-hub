variable name {
  type        = string
  description = "(Required) Specifies the name of the ServiceBus Queue resource. Changing this forces a new resource to be created."
}

variable namespace_name {
  type        = string
  description = "(Required) The name of the ServiceBus Namespace to create this queue in. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the namespace. Changing this forces a new resource to be created."
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
