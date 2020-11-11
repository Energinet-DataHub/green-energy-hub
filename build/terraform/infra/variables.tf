variable "appname" {
  type = string
  default = "energinetdh"
}

variable "resource_group_name" {
  type = string
  default = "rg-GreenEnergyHub_Sandbox-S"
}

variable "location" {
  type = string
  default = "North Europe"
}

variable "environment" {
  type = string
  default = "test"
}

variable "eventhub_namespace_name" {
  type = string
  default = "energinet-eh-ns"
}

variable "input_eventhub_name" {
  type = string
  default = "input_poc"
}

variable "output_eventhub_name" {
  type = string
  default = "output_poc"
}

variable "storage_account_name" {
  type = string
  default = "energinetstrg1"
}

variable "container_name" {
  type = string
  default = "messagedata"
}
