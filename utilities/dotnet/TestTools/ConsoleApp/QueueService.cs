using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal class QueueService : IHostedService
{
    private ILogger<QueueService> _logger { get; }
    private IHostApplicationLifetime _appLifetime  { get; }
    private IConfiguration _config { get; }

    public QueueService(ILogger<QueueService> logger, IHostApplicationLifetime appLifetime, IConfiguration config)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _config = config;
    }

    public async Task DoWork()
    {
        var ehConnectionString = _config.GetSection("EventHub").GetValue<string>("InputConnectionString");
        var messageCount = _config.GetValue<int>("MessageCount");

        // Create a producer client that you can use to send events to an event hub
        await using (var producerClient = new EventHubProducerClient(ehConnectionString))
        {
            var random = new Random();
            var amountLeft = messageCount;
            int batchCount = 0;
            while (amountLeft > 0)
            {
                // Create a batch of events and continue adding until no longer able
                batchCount = 0;
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                while (amountLeft > 0)
                {
                    var meterReadDate = DateTime.UtcNow.ToString("O");
                    var jsonContent = $"{{\"customerId\":{random.Next(-5, 15)},\"meterId\":{random.Next(-5, 15)},\"meterValue\":{random.Next(-5, 15)},\"meterReadDate\":\"{meterReadDate}\"}}";
                    var bytes = Encoding.UTF8.GetBytes(jsonContent);
                    // Add events to the batch. An event is a represented by a collection of bytes and metadata.
                    if (!eventBatch.TryAdd(new EventData(bytes)))
                    {
                        break;
                    }
                    batchCount++;
                    amountLeft--;
                }
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                if (amountLeft == 0)
                {
                    _logger.LogInformation($"Final flush - A batch of {batchCount} events has been published.");
                }
                else
                {
                    _logger.LogInformation($"A batch of {batchCount} events has been published.");
                }
            }
        }

        await Task.Delay(1000);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Register hooks
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("OnStarted has been called.");

        Task.Run(DoWork).Wait(); // fixme deadlocks possible

        _appLifetime.StopApplication();
    }

    private void OnStopping()
    {
        _logger.LogInformation("OnStopping called.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("OnStopped called.");
    }
}