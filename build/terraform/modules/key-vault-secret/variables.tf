variable name {
  type        = string
  description = "(Required) Specifies the name of the Key Vault Secret. Changing this forces a new resource to be created."
}

variable "value" {
  type        = string
  description = "(Required) Specifies the value of the Key Vault Secret."
}

variable "key_vault_id" {
  type        = string
  description = "(Required) The ID of the Key Vault where the Secret should be created."
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
