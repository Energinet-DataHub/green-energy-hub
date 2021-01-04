// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Text;
using TimeSeriesGenerator;
using TimeSeriesGenerator.Domain;
using TimeSeriesGeneratorApp;
using Utf8Json;
using EventData = Azure.Messaging.EventHubs.EventData;

internal class QueueService : IHostedService
{
    private readonly ITimeSeriesGeneratorService _timeSeriesGeneratorService;
    private readonly EventHubProducerClient _eventHubProducerClient;
    private ConcurrentDictionary<int, int> _stackSizes;
    private bool _stop;
    private ConcurrentDictionary<int, double> _throughputs;
    private int _meteringPointsPerGridArea;
    private int _numberOfGridAreas;
    private int _numberOfMeteringPoints;

    public QueueService(
        ILogger<QueueService> logger,
        IHostApplicationLifetime appLifetime,
        IConfiguration config,
        ITimeSeriesGeneratorService timeSeriesGeneratorService,
        EventHubProducerClient eventHubProducerClient)
    {
        _timeSeriesGeneratorService = timeSeriesGeneratorService;
        _eventHubProducerClient = eventHubProducerClient;
        Logger = logger;
        AppLifetime = appLifetime;
        Config = config;
    }

    private ILogger<QueueService> Logger { get; }

    private IHostApplicationLifetime AppLifetime { get; }

    private IConfiguration Config { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Run(
            () =>
        {
            // Register hooks
            AppLifetime.ApplicationStarted.Register(OnStarted);
            AppLifetime.ApplicationStopping.Register(OnStopping);
            AppLifetime.ApplicationStopped.Register(OnStopped);
        }, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }

    public void DoWork()
    {
        try
        {
            _throughputs = new ConcurrentDictionary<int, double>();
            _stackSizes = new ConcurrentDictionary<int, int>();
            _meteringPointsPerGridArea = Config.GetValue<int>("MeteringPointsPerGridArea");
            _numberOfGridAreas = Config.GetValue<int>("NumberOfGridAreas");
            _numberOfMeteringPoints = _numberOfGridAreas * _meteringPointsPerGridArea;
            var csvFile = Config.GetValue<string>("DistributionOfMeteringPointsCsvFileName");
            var resolution = Config.GetValue<int>("MinutesPerTimeSeriesPoint");
            var start = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd").Parse(Config.GetValue<string>("StartDate")).Value;
            var end = InstantPattern.CreateWithInvariantCulture("yyyy-MM-dd").Parse(Config.GetValue<string>("EndDate")).Value;
            var generatedTimeSpanSet = _timeSeriesGeneratorService.GenerateTimeSpans(start, end, resolution);

            if (string.IsNullOrWhiteSpace(csvFile))
            {
                GenerateAndRunBasedOnApproximateMethod(generatedTimeSpanSet);
            }
            else
            {
                GenerateAndRunBasedOfCsvFile(csvFile, generatedTimeSpanSet);
            }
        }
        catch (Exception e)
        {
            Logger.LogCritical("Failure", e);
            throw;
        }
    }

    private void GenerateAndRunBasedOnApproximateMethod(List<Instant> generatedTimeSpanSet)
    {
        var exchange =
            _timeSeriesGeneratorService.ExchangeDataset(generatedTimeSpanSet, _numberOfMeteringPoints);
        var production =
            _timeSeriesGeneratorService.ProductionDataset(generatedTimeSpanSet, _numberOfMeteringPoints);
        var consumption =
            _timeSeriesGeneratorService.ConsumptionDataset(generatedTimeSpanSet, _numberOfGridAreas, _meteringPointsPerGridArea, _numberOfMeteringPoints);

        SendPerTU(exchange, "Exchange dataset");
        SendPerTU(production, "Production dataset");
        SendPerTU(consumption, "Consumption dataset");
    }

    private void GenerateAndRunBasedOfCsvFile(string csvFile, List<Instant> generatedTimeSpanSet)
    {
        var timeSeriesSetFromCsv =
            _timeSeriesGeneratorService.GenerateTimeSeriesFromCsvFile(csvFile, generatedTimeSpanSet, _numberOfMeteringPoints);
        SendPerTU(timeSeriesSetFromCsv, "Csv file");
    }

    private void SendPerTU(TimeSeriesPoint[] source, string setName)
    {
        Logger.LogInformation($"we are sending {source.Length} time series values for {setName}");
        //concurrentSenders should match TUS in the eventhub NS
        var concurrentSenders = Config.GetValue<int>("ConcurrentSenders");
        Logger.LogInformation($"Dividing the work into {concurrentSenders} threads");
        var splitSize = source.Length / concurrentSenders;
        var splits = source.Split(splitSize);
        var tasks = new List<Task>();

        foreach (var split in splits)
        {
            tasks.Add(Task.Run(() => ThreadForEachEventHubTransferUnitAsync(split)));
        }

        tasks.Add(Task.Run(MonitoringThread));

        Task.WaitAll(tasks.ToArray());
        Logger.LogInformation("All threads complete");
    }

    private void MonitoringThread()
    {
        Logger.LogInformation($"Warming up the monitor");
        //Allow the other threads to start before we measure
        System.Threading.Thread.Sleep(5000);
        while (!_stop && _throughputs.Any())
        {
            var sum = _throughputs.Sum(t => t.Value);
            var msgSum = _stackSizes.Sum(t => t.Value);
            Logger.LogInformation($"Current Throughput is around {sum:N0} msg/sec we have {msgSum:N0} left");
            System.Threading.Thread.Sleep(1000);
        }
    }

    private async Task ThreadForEachEventHubTransferUnitAsync(IEnumerable<TimeSeriesPoint> source)
    {
        Logger.LogInformation("Starting partitioning thread");
        var timeSeriesPoints = new Stack<TimeSeriesPoint>(source);
        var currentThreadManagedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
        //prepare the dictionary for use in the loop
        _throughputs.TryAdd(currentThreadManagedThreadId, 0);
        _stackSizes.TryAdd(currentThreadManagedThreadId, 0);

        while (timeSeriesPoints.TryPeek(out var result))
        {
            await ChunkTaskAsync(timeSeriesPoints, _eventHubProducerClient);
            _stackSizes[currentThreadManagedThreadId] = timeSeriesPoints.Count;
            if (_stop)
            {
                break;
            }
        }

        _throughputs.Clear();
    }

    private async Task ChunkTaskAsync(Stack<TimeSeriesPoint> timeSeriesPoints, EventHubProducerClient producerClient)
    {
        var sw = new Stopwatch();
        var chunkSize = 0; // 950 is around the amount we can add to the batch
        sw.Start();
        try
        {
            using (var eventBatch = await producerClient.CreateBatchAsync())
            {
                while (timeSeriesPoints.TryPop(out var chunk))
                {
                    //We are using utf8json here since it is faster than the standard text.json in this case
                    var bytes = JsonSerializer.Serialize(chunk);
                    // Add events to the batch. An event is a represented by a collection of bytes and metadata.
                    if (!eventBatch.TryAdd(new EventData(bytes)))
                    {
                        break;
                    }

                    chunkSize++;
                }

                try
                {
                    await producerClient.SendAsync(eventBatch);
                }
                catch (EventHubsException e)
                {
                    if (e.Reason == EventHubsException.FailureReason.QuotaExceeded)
                    {
                        Logger.LogWarning("Pushing it to hard. Consider increasing the TU of the EventHub", e);
                        //Wait a bit. If these come to often increase the TU
                        System.Threading.Thread.Sleep(4000);
                    }
                    else
                    {
                        Logger.LogError(e, "An error occured when sending to the eventhub");
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e, "bah");
                }

                sw.Stop();
                var throughput = chunkSize / sw.Elapsed.TotalSeconds;
                var tid = System.Threading.Thread.CurrentThread.ManagedThreadId;

                _throughputs[tid] = throughput;
                //Logger.LogInformation($"Throughput of task {} is {throughput:F0} msg/s");
            }
        }
        catch (Exception e)
        {
            Logger.LogError("General error in ChunkTaskAsync", e);
            throw;
        }
    }

    private void OnStarted()
    {
        Logger.LogInformation("OnStarted has been called.");

        Task.Run(DoWork).Wait(); // fixme deadlocks possible

        AppLifetime.StopApplication();
    }

    private void OnStopping()
    {
        _stop = true;
        Logger.LogInformation("OnStopping called.");
    }

    private void OnStopped()
    {
        Logger.LogInformation("OnStopped called.");
    }
}
