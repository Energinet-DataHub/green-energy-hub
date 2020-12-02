#Keeping secret for future secrets handling optimizaiton
#resource "databricks_secret_scope" "energinet_poc" {
#  name = "POC_Secret_Scope"
#}

#resource "databricks_secret" "eventhub_connection" {
#  key          = "eventhub_connection"
#  string_value = var.input_eh_listen_connection_string
#  scope        = databricks_secret_scope.energinet_poc.name
#}

data "azurerm_key_vault_secret" "appinsights_instrumentation_key" {
  name         = "appinsights-instrumentation-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "storage_account_key" {
  name         = "storage-account-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "input_eventhub_listen_connection_string" {
  name         = "input-eventhub-listen-connection-string"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "valid_output_eventhub_send_connection_string" {
  name         = "valid-output-eventhub-send-connection-string"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "invalid_output_eventhub_send_connection_string" {
  name         = "invalid-output-eventhub-send-connection-string"
  key_vault_id = var.keyvault_id
}

resource "databricks_job" "streaming_job" {
  name = "StreamProcessing"
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
      "--storage-account-key=${data.azurerm_key_vault_secret.storage_account_key.value}",
      "--storage-container-name=${var.streaming_container_name}",
      "--master-data-path=master-data/master-data.csv",
      "--output-path=delta/meter-data/",
      "--input-eh-connection-string=${data.azurerm_key_vault_secret.input_eventhub_listen_connection_string.value}",
      "--max-events-per-trigger=1000",
      "--trigger-interval=1 second",
      "--streaming-checkpoint-path=checkpoints/streaming",
      "--valid-output-eh-connection-string=${data.azurerm_key_vault_secret.valid_output_eventhub_send_connection_string.value}",
      "--invalid-output-eh-connection-string=${data.azurerm_key_vault_secret.invalid_output_eventhub_send_connection_string.value}",
      "--telemetry-instrumentation-key=${data.azurerm_key_vault_secret.appinsights_instrumentation_key.value}"
    ]
  }

  email_notifications {
    no_alert_for_skipped_runs = true
  }
}