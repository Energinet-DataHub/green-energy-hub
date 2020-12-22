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

variable "cosmosdb-database-name" {
  type = string
}

variable "cosmosdb-collection-name" {
  type = string
}

variable "cosmosdb-account-endpoint" {
  type = string
}

variable "cosmosdb-account-primary-key" {
  type = string
}

variable "appinsights_instrumentation_key" {
  type = string
}

variable "wheel_file" {
  type = string
}

variable "custom_cosmosdb_connector" {
  type = string
  default = "dbfs:/streaming/cosmosdb-connector.jar"
}

variable "python_main_file" {
  type = string
}
