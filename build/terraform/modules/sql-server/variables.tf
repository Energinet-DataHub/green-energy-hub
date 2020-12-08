variable name {
  type        = string
  description = "(Required) The name of the Microsoft SQL Server. This needs to be globally unique within Azure."
}

variable resource_group_name {
  type        = string
  description = "(Required) The name of the resource group in which to create the Microsoft SQL Server."
}

variable location {
  type        = string
  description = "(Required) Specifies the supported Azure location where the resource exists. Changing this forces a new resource to be created."
}

variable sql_version {
  type        = string
  description = "(Required) The version for the new server. Valid values are: 2.0 (for v11 server) and 12.0 (for v12 server).The Azure SQL Server version."
  default     = "12.0"
}

variable administrator_login {
  type        = string
  description = "(Required) The administrator login name for the new server. Changing this forces a new resource to be created."
}

variable administrator_login_password {
  type        = string
  description = "(Required) The password associated with the administrator_login user. Needs to comply with Azure's Password Policy"
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
