variable name {
  type        = string
  description = "(Required) Specifies the name of the SQL Linked service. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the SQL Linked service. Changing this forces a new resource."
}

variable data_factory_name {
  type        = string
  description = "(Required) The Data Factory name in which to associate the Linked service with. Changing this forces a new resource."
}

variable connection_string {
  type        = string
  description = "(Optional) The connection string. Required if account_endpoint, account_key, and database are unspecified."
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}