using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using ValidatorTool.RuleEngines.MSRE;
using ValidatorTool.RuleEngines.NRules;

internal class BenchmarkService : IHostedService
{
    private ILogger<BenchmarkService> _logger { get; }
    private IHostApplicationLifetime _appLifetime  { get; }
    private IConfiguration _config { get; }

    private IRuleEngine _msre;
    private IRuleEngine _nrules;
    private IRuleEngine _fluent;

    private static readonly int BENCHMARK_SIZE = 100000;

    public BenchmarkService(ILogger<BenchmarkService> logger, IHostApplicationLifetime appLifetime, IConfiguration config)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _config = config;

        var storage = _config.GetSection("Storage");
        var blobStorage = new BlobWorkflowRulesStorage(storage.GetValue<string>("OutputConnectionString"), storage.GetValue<string>("RulesContainerName"), storage.GetValue<string>("RulesBlobName"));
        _msre = new MSREEngine(blobStorage);
        _nrules = new NRulesEngine();
        _fluent = new FluentValidationEngine();
    }

    public async Task DoWork()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        _logger.LogInformation("Generating messages...");
        // Generate in-memory list of meter messages
        var messages = new List<MeterMessage>(BENCHMARK_SIZE);
        var random = new Random();
        for (int i=0; i < BENCHMARK_SIZE; i++) {
            var customerId = random.Next(-5, 15);
            var meterId = random.Next(-5, 15);
            var meterValue = random.Next(-5, 15);
            var meterReadDate = DateTime.UtcNow;
            messages.Add(new MeterMessage(meterValue, meterId, meterReadDate, customerId));
        }
        sw.Stop();
        _logger.LogInformation("Messages generated in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

        // Run the benchmarks
        _logger.LogInformation("*** BATCH");
        sw.Start();
        _nrules.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("NRules ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        sw.Reset();


        sw.Start();
        _fluent.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("Fluent ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        sw.Reset();
        sw.Start();
        //_msre.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("MSRE ran in {0}ms", sw.Elapsed.TotalMilliseconds);


        _logger.LogInformation("*** SEQUENTIAL");
        sw.Start();
        foreach(var message in messages)
        {
            _nrules.ValidateAsync(message);
        }
        sw.Stop();
        _logger.LogInformation("NRules ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        sw.Reset();

        sw.Start();
        foreach(var message in messages)
        {
            _msre.ValidateAsync(message);
        }
        sw.Stop();
        _logger.LogInformation("MSRE ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        sw.Start();
        foreach(var message in messages)
        {
            _fluent.ValidateAsync(message);
        }
        sw.Stop();
        _logger.LogInformation("Fluent ran in {0}ms", sw.Elapsed.TotalMilliseconds);



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