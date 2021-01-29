variable name {
  type        = string
  description = "(Required) Specifies the name of the Data Factory Pipeline. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the Data Factory Pipeline. Changing this forces a new resource."
}

variable data_factory_name {
  type        = string
  description = "(Required) The Data Factory name in which to associate the Pipeline with. Changing this forces a new resource."
}

variable activities_json {
  type        = string
  description = "(Optional) A JSON object that contains the activities that will be associated with the Data Factory Pipeline."
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}