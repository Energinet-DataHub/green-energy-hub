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
  description   = "Enviroment that the infrastructure code is deployed into"
}

variable "organisation" {
  type          = string
  description   = "Organisation that is running the infrastructure code"
}

variable "current_spn_object_id" {
  type          = string
  description   = "Service Principal ID of the connection used to deploy the code"
}

variable "current_tenant_id" {
  type          = string
  description   = "Tenant Id that the infrastructure code is deployed into"
}
