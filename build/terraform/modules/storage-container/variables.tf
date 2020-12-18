variable container_name {
  type        = string
  description = "(Required) Specifies the name of the storage container name. Changing this forces a new resource to be created."
}

variable storage_account_name {
  type        = string
  description = "(Required) The name of the storage account in which container should be created."
}

variable container_access_type {
  type        = string
  description = "(Required) Acces type for container"
}

variable dependencies {
  type        = list
  description = "A mapping of dependencies which this module depends on."
  default     = []
}