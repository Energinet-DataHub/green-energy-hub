module "streaming_job" {
  source                                         = "../job_modules/streaming_job"
  databricks_id                                  = var.databricks_id
  module_name                                    = "StreamingJob"
  storage_account_name                           = var.storage_account_name
  storage_account_key                            = var.storage_account_key
  streaming_container_name                       = var.streaming_container_name
  input_eventhub_listen_connection_string        = var.input_eventhub_listen_connection_string
  valid_output_eventhub_send_connection_string   = var.valid_output_eventhub_send_connection_string
  invalid_output_eventhub_send_connection_string = var.invalid_output_eventhub_send_connection_string 
  appinsights_instrumentation_key                = var.appinsights_instrumentation_key
  wheel_file                                     = var.wheel_file
  python_main_file                               = var.python_main_file
}
