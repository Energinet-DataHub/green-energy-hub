variable name {
  type        = string
  description = "(Required) Specifies the name of the EventGrid Topic resource. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which the EventGrid Topic exists. Changing this forces a new resource to be created."
}

variable location {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
}

variable input_schema {
  type        = string
  description = "(Optional) Specifies the schema in which incoming events will be published to this domain. Allowed values are CloudEventSchemaV1_0, CustomEventSchema, or EventGridSchema. Defaults to EventGridSchema. Changing this forces a new resource to be created."
  default     = "EventGridSchema"
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