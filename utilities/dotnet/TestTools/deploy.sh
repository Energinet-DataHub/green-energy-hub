#!/bin/bash

# Set these as appropriate
RG="geh-test"
EH_NS="${RG}-ns"
EH_INPUT_NAME="input"
EH_OUTPUT_NAME="output"
STORAGE_NAME="gehtest"
RULES_CONTAINER="gehtest"
PROCESSOR_CONTAINER="processorcontainer"
STORAGE_BLOB_NAME="ValidationRules.json"

az group create -l westus2 -n "$RG"

# Event hubs
az eventhubs namespace create -g "$RG" -n "$EH_NS"
az eventhubs eventhub create -g "$RG" -n "$EH_INPUT_NAME" --namespace "$EH_NS"
az eventhubs eventhub create -g "$RG" -n "$EH_OUTPUT_NAME" --namespace "$EH_NS"

# ...for ConsoleApp
az eventhubs eventhub authorization-rule create -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_INPUT_NAME" -n "InputWrite" --rights Send
az eventhubs eventhub authorization-rule create -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_OUTPUT_NAME" -n "OutputRead" --rights Listen

# ...for ValidatorTool
az eventhubs eventhub authorization-rule create -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_INPUT_NAME" -n "InputRead" --rights Listen
az eventhubs eventhub authorization-rule create -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_OUTPUT_NAME" -n "OutputWrite" --rights Send

# Storage (both tools)
az storage account create -g "$RG" -n "$STORAGE_NAME"
az storage container create --account-name "$STORAGE_NAME" -n "$RULES_CONTAINER"
az storage container create --account-name "$STORAGE_NAME" -n "$PROCESSOR_CONTAINER"
az storage blob upload -f ./ValidatorTool/RuleEngines/MSRE/ValidationRules.json --account-name "$STORAGE_NAME" -c "$RULES_CONTAINER" -n "$STORAGE_BLOB_NAME"

# Print resulting secrets secrets
echo "*** Storage connection string: $(az storage account show-connection-string -n "$STORAGE_NAME" -o tsv)"
echo "*** Rules storage container name: $RULES_CONTAINER"
echo "*** Rules storage blob name: $STORAGE_BLOB_NAME"
echo "*** Event Hub Processor container name: $PROCESSOR_CONTAINER"
echo "*** ConsoleApp event hubs"
echo "- Input: $(az eventhubs eventhub authorization-rule keys list -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_INPUT_NAME" -n InputWrite -o tsv --query primaryConnectionString)"
echo "- Output: $(az eventhubs eventhub authorization-rule keys list -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_OUTPUT_NAME" -n OutputRead -o tsv --query primaryConnectionString)"

echo "*** ValidatorTool event hubs"
echo "- Input: $(az eventhubs eventhub authorization-rule keys list -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_INPUT_NAME" -n InputRead -o tsv --query primaryConnectionString)"
echo "- Output: $(az eventhubs eventhub authorization-rule keys list -g "$RG" --namespace-name "$EH_NS" --eventhub-name "$EH_OUTPUT_NAME" -n OutputWrite -o tsv --query primaryConnectionString)"
