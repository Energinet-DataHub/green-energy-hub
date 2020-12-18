resource "databricks_job" "streaming_job" {
  name = var.module_name
  max_retries = 2
  max_concurrent_runs = 1

  new_cluster { 
    spark_version  = "7.2.x-scala2.12"
    node_type_id   = "Standard_DS3_v2"
    num_workers    = 1
  }
	
  library {
    maven {
      coordinates = "com.microsoft.azure:azure-eventhubs-spark_2.12:2.3.17"
    }
  }

  library {
    pypi {
      package = "configargparse==1.2.3"
    }
  }

  library {
    pypi {
      package = "applicationinsights==0.11.9"
    }
  } 

  library {
    whl = var.wheel_file
  } 

  spark_python_task {
    python_file = var.python_main_file
    parameters  = [
      "--storage-account-name=${var.storage_account_name}",
      "--storage-account-key=${var.storage_account_key}",
      "--storage-container-name=${var.streaming_container_name}",
      "--master-data-path=master-data/master-data.csv",
      "--output-path=delta/meter-data/",
      "--input-eh-connection-string=${var.input_eventhub_listen_connection_string}",
      "--max-events-per-trigger=1000",
      "--trigger-interval=1 second",
      "--streaming-checkpoint-path=checkpoints/streaming",
      "--valid-output-eh-connection-string=${var.valid_output_eventhub_send_connection_string}",
      "--invalid-output-eh-connection-string=${var.invalid_output_eventhub_send_connection_string}",
      "--telemetry-instrumentation-key=${var.appinsights_instrumentation_key}",
      "--cosmosdb-account-endpoint=${var.cosmosdb-account-endpoint}",
      "--cosmosdb-account-primary-key=${var.cosmosdb-account-primary-key}"
    ]
  }

  email_notifications {
    no_alert_for_skipped_runs = true
  }
}
