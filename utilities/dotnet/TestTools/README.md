# Test tools for the ingestion via eventhub

## General

These tools supports enqueueing messages onto an 'input' Event Hub (via ConsoleApp), where the randomly-generated meter messages land as JSON strings.

An Azure Function in the ValidatorTool project will read these messages from the queue, deserialize them, and submit them to a rules engine as configured in local.settings.json. Once the validation results are retrieved from the configured rules engine, they are submitted to an output Event Hub which can be monitored for results (via ConsoleApp).

It requires some Azure resources:

* An Event Hub namespace with an input and output event hub
* A blob storage account

ConsoleApp can be executed with dotnet run with either the `enqueue` or `output` arguments. See the [ConsoleApp/README.md](ConsoleApp/README.md) file for more details. It requires write access to the input Event Hub and read access to the output Event Hub.

ValidatorTool can be executed with `func host start` and requires the connection string, container name and blob name pointing to where the MS Rules Engine rule set was uploaded (source data available at `ValidatorTool/RuleEngines/MSRE/ValidationRules.json`), as well as read access to the input Event Hub and write access to the output Event Hub (the inverse permissions of the console app).

## Deployment

1. Copy `ConsoleApp/appsettings.json.sample` to `ConsoleApp/appsettings.Development.json`
2. Copy `ValidatorTool/local.settings.json.sample` to `ValidatorTool/local.settings.json`
3. Edit the variable header block in `deploy.sh` as desired, and then execute it.
   Before executing, make sure that you are logged into the Azure CLI and set your default subscription.
4. Copy the configuration secrets printed into the local configuration files
