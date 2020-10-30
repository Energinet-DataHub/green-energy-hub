# Console app

This console app submits messages to the input queue or reads them back from the output queue.

## Setup

Copy the reference `appsettings.json.sample` to `appsettings.Development.json` and fill in the required fields by running the `deploy.sh` script and copying in the values printed there.

It requires an Event Hub namespace with two Event Hubs (`input` with a write access policy, and `output` with a read access policy) - these will be printed under the section "ConsoleApp event hubs".

Checkpointing on the output queue also means we need a storage account + container. Copy the printed storage connection string into as the "OutputConnectionString" and the Event Hub Processor storage container name into "EventProcessorContainerName" in `appsettings.json`.

The benchmarking service also instantiates a RulesEngine (MSRE) instance, which requires a separate storage container and .json file (which will be in the same storage account as specified by the above "OutputConnectionString") - copy the printed rules storage container name into "RulesContainerName" and the rules storage blob name into "RulesBlobName" in `appsettings.json`.

## Usage

To run this sample, choose between 'enqueue' (send new messages to the input queue) or 'output' (read messages from output queue).

For enqueue, set `MessageCount` as desired in `appsettings.Development.json` and run:

```sh
DOTNET_ENVIRONMENT=Development dotnet run enqueue
```

The program will terminate upon submission of the specified number of messages.

For output monitoring mode, run:

```sh
DOTNET_ENVIRONMENT=Development dotnet run output
```

The application will print any **new** messages since the last checkpoint (and checkpoint every time a message is read). Ctrl+C exits the program.

If you are debugging in VS Code, Debug Console does not pass Ctrl+C. Execute this to terminate the program:

```sh
kill -s TERM $(pgrep -f ConsoleApp.dll)
```
