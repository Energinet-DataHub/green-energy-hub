data "azurerm_key_vault_secret" "appinsights_instrumentation_key" {
  name         = "appinsights-instrumentation-key"
  key_vault_id = var.keyvault_id
}

data "azurerm_key_vault_secret" "storage_account_key" {
  name         = "storage-account-key"
  key_vault_id = var.keyvault_id
}

resource "databricks_job" "aggregation_job" {
  name = "AggregationJob"
  max_retries = 0
  max_concurrent_runs = 0

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
      "--input-storage-account-name=${var.storage_name}",
      "--input-storage-account-key=${data.azurerm_key_vault_secret.storage_account_key.value}",
      "--input-storage-container-name=${var.streaming_container_name}",
      "--output-storage-account-name=${var.storage_name}",
      "--output-storage-account-key=${data.azurerm_key_vault_secret.storage_account_key.value}",
      "--output-storage-container-name=${var.aggregation_container_name}",
      "--output-path=delta/hourly-consumption/",
      "--beginning-date-time=${var.beginning_date_time}",
      "--end-date-time=${var.end_date_time}",
      "--telemetry-instrumentation-key=${data.azurerm_key_vault_secret.appinsights_instrumentation_key.value}"
    ]
  }

  email_notifications {
    no_alert_for_skipped_runs = true
  }
}
