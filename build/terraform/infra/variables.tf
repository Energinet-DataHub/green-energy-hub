variable "databricks_name" {
  type = string
}

variable "resource_group_name" {
  type = string
}

variable "keyvault_name" {
  type = string
}

variable "location" {
  type = string
  default = "North Europe"
}

variable "eventhub_namespace_name" {
  type = string
  default = "energinet-ehn"
}

variable "input_eventhub_name" {
  type = string
  default = "input"
}

variable "valid_output_eventhub_name" {
  type = string
  default = "valid-output"
}

variable "invalid_output_eventhub_name" {
  type = string
  default = "invalid-output"
}

variable "storage_account_name" {
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

variable "tenant_id" {
  type = string
}

variable "object_id" {
  type = string
}

variable "appinsights_name" {
  type = string
}