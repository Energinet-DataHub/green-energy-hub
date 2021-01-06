variable name {
  type        = string
  description = "(Required) The name of the API Management Service. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the Resource Group in which the API Management Service should be exist. Changing this forces a new resource to be created."
}

variable location {
  type        = string
  description = "(Required) The Azure location where the API Management Service exists. Changing this forces a new resource to be created."
}

variable publisher_name {
  type        = string
  description = "(Required) The name of publisher/company."
}

variable publisher_email {
  type        = string
  description = "(Required) The email of publisher/company."
}

variable sku_name {
  type        = string
  description = "(Required) sku_name is a string consisting of two parts separated by an underscore(_). The first part is the name, valid values include: Consumption, Developer, Basic, Standard and Premium. The second part is the capacity (e.g. the number of deployed units of the sku), which must be a positive integer (e.g. Developer_1)."
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