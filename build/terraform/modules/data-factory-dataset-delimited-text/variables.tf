variable name {
  type        = string
  description = "(Required) Specifies the name of the delimited text dataset. Changing this forces a new resource to be created."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the delimited text dataset. Changing this forces a new resource."
}

variable data_factory_name {
  type        = string
  description = "(Required) The Data Factory name in which to associate the delimited text dataset with. Changing this forces a new resource."
}

variable linked_service_name {
  type        = string
  description = "(Required) The Data Factory Linked Service name in which to associate the Dataset with."
}

variable column_delimiter {
  type        = string
  description = "(Required) The column delimiter."
}

variable row_delimiter {
  type        = string
  description = "(Required) The row delimiter."
}

variable encoding {
  type        = string
  description = "(Required) The encoding format for the file."
}

variable quote_character {
  type        = string
  description = "(Required) The quote character."
}

variable escape_character {
  type        = string
  description = "(Required) The escape character."
}

variable first_row_as_header {
  type        = bool
  description = "(Required) When used as input, treat the first row of data as headers. When used as output, write the headers into the output as the first row of data."
}

variable null_value {
  type        = string
  description = "(Required) The null value string."
}

variable azure_blob_storage_location {
  type        = any
  description = "(Required) The null value string."
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}