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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using ValidatorTool.RuleEngines.MSRE;
using ValidatorTool.RuleEngines.NRules;

internal class BenchmarkService : IHostedService
{
    private static readonly int _BenchmarkSize = 10000;

    private readonly ILogger<BenchmarkService> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IRuleEngine _msre;
    private readonly IRuleEngine _nrules;
    private readonly IRuleEngine _fluent;

    public BenchmarkService(ILogger<BenchmarkService> logger, IHostApplicationLifetime appLifetime, IWorkflowRulesStorage blobStorage)
    {
        _logger = logger;
        _appLifetime = appLifetime;

        _msre = new MSREEngine(blobStorage);
        _nrules = new NRulesEngine();
        _fluent = new FluentValidationEngine();
    }

    public async Task DoWorkAsync()
    {
        var sw = new Stopwatch();
        sw.Start();
        _logger.LogInformation("Generating messages...");
        // Generate in-memory list of meter messages
        var messages = new List<MeterMessage>(_BenchmarkSize);
        var random = new Random();
        for (var i = 0; i < _BenchmarkSize; i++)
        {
            var customerId = random.Next(-5, 15);
            var meterId = random.Next(-5, 15);
            var meterValue = random.Next(-5, 15);
            var meterReadDate = DateTime.UtcNow;
            messages.Add(new MeterMessage(meterValue, meterId, meterReadDate, customerId));
        }

        sw.Stop();
        _logger.LogInformation("Messages generated in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

        /*
         * Batch run benchmarks
         */
        _logger.LogInformation("*** BATCH");

        // NRules
        sw.Start();
        await _nrules.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("NRules ran in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

        // Fluent
        sw.Start();
        await _fluent.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("Fluent ran in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

        // MSRE
        sw.Start();
        await _msre.ValidateBatchAsync(messages);
        sw.Stop();
        _logger.LogInformation("MSRE ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        /*
         * Sequential Benchmarks
         */
        _logger.LogInformation("*** SEQUENTIAL");

        // NRules
        sw.Start();
        foreach (var message in messages)
        {
            await _nrules.ValidateAsync(message);
        }

        sw.Stop();
        _logger.LogInformation("NRules ran in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

        // Fluent
        sw.Start();
        foreach (var message in messages)
        {
            await _fluent.ValidateAsync(message);
        }

        sw.Stop();
        _logger.LogInformation("Fluent ran in {0}ms", sw.Elapsed.TotalMilliseconds);

        // MSRE
        sw.Start();
        foreach (var message in messages)
        {
            await _msre.ValidateAsync(message);
        }

        sw.Stop();
        _logger.LogInformation("MSRE ran in {0}ms", sw.Elapsed.TotalMilliseconds);
        sw.Reset();

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

        Task.Run(DoWorkAsync).Wait();

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
