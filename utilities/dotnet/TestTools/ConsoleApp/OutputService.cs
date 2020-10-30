using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class OutputService : IHostedService
{
    private ILogger<OutputService> _logger { get; }
    private IConfiguration _config { get; }
    private EventProcessorClient _processor { get; set; }

    public OutputService(ILogger<OutputService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        // Write the body of the event to the console window
        _logger.LogInformation("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

        // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        _logger.LogInformation($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        _logger.LogInformation(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ehConnectionString = _config.GetSection("EventHub").GetValue<string>("OutputConnectionString");
        var blobConnectionString = _config.GetSection("Storage").GetValue<string>("OutputConnectionString");
        var blobContainerName = _config.GetSection("Storage").GetValue<string>("EventProcessorContainerName");

        // Read from the default consumer group: $Default
        string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        // Create a blob container client that the event processor will use
        BlobContainerClient storageClient = new BlobContainerClient(blobConnectionString, blobContainerName);

        // Create an event processor client to process events in the event hub
        var options = new EventProcessorClientOptions() { MaximumWaitTime = TimeSpan.FromSeconds(1) };
        _processor = new EventProcessorClient(storageClient, consumerGroup, ehConnectionString, options);

        // Register handlers for processing events and handling errors
        _processor.ProcessEventAsync += ProcessEventHandler;
        _processor.ProcessErrorAsync += ProcessErrorHandler;

        // Start the processing
        await _processor.StartProcessingAsync();

        _logger.LogInformation("Starting to process events");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop the processing
        _logger.LogInformation("Stopping event processor, please wait...");
        await _processor.StopProcessingAsync();
        _logger.LogInformation("Event processor stopped");
    }
}