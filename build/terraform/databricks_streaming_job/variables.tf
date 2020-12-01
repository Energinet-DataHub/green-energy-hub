variable "databricks_id" {
  type = string
}

variable "storage_account_name" {
  type = string
}

variable "streaming_container_name" {
  type = string
  default = "messagedata"
}

variable "python_main_file" {
  type = string
}

variable "wheel_file" {
  type = string
}

variable "keyvault_id" {
  type = string
}



