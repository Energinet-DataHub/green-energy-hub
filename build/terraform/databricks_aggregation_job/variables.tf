variable "databricks_id" {
  type = string
}

variable "storage_name" {
  type = string
  default = "energinetstrg1"
}

variable "streaming_container_name" {
  type = string
  default = "messagedata"
}

variable "aggregation_container_name" {
  type = string
  default = "aggregations"
}

variable "beginning_date_time" {
  type = string
  default = "2020-10-02T00:00:00+0100"
}

variable "end_date_time" {
  type = string
  default = "2020-10-03T00:00:00+0100"
}

variable "python_main_file" {
  type = string
}

variable "wheel_file" {
  type = string
}

variable "telemetry_instrumentation_key" {
  type = string
}

variable "keyvault_id" {
  type = string
}




