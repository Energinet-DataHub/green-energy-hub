variable "module_name" {
  type = string
}

variable "databricks_id" {
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "storage_account_key" {
  type = string
}

variable "streaming_container_name" {
  type = string
}

variable "input_eventhub_listen_connection_string" {
  type = string
}

variable "valid_output_eventhub_send_connection_string" {
  type = string
}

variable "invalid_output_eventhub_send_connection_string" {
  type = string
}

variable "appinsights_instrumentation_key" {
  type = string
}

variable "wheel_file" {
  type = string
}

variable "python_main_file" {
  type = string
}
