# Eventhub Test Tools

## General

This app supports enqueueing messages onto an 'input' Event Hub (via ConsoleApp), where the randomly-generated meter messages land as JSON strings.

It requires an Event Hub namespace with an input event hub.

ConsoleApp can be executed with `dotnet run`.
It requires write access to the input Event Hub.

## Deployment

1. Copy `TimeSeriesGenerator/TimeSeriesGeneratorApp/appsettings.json.sample` to `TimeSeriesGenerator/TimeSeriesGeneratorApp/appsettings.Development.json`
2. Set the settings as per sample file.
