variable "resource_group_name" {
  type = string
}

variable "streaming_container_name" {
  type = string
  default = "messagedata"
}

variable "aggregation_container_name"{
  type = string
  default = "aggregations"
}

variable "environment" {
  type          = string
  description   = "Enviroment"
}

variable "current_spn_object_id" {
  type          = string
  description   = "Service Principal ID"
}

variable "current_tenant_id" {
  type          = string
  description   = "tenant Id"
}

variable "env_count" {
  type          = number
  description   = "Count of Environments to Create"
  default       = 3
}
