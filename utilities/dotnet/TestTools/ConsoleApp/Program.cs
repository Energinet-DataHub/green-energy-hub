using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ValidatorTool.RuleEngines.MSRE;

namespace ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Program.cs operation_mode");
                Console.WriteLine("Error: specify an operation_mode of either 'enqueue' or 'output'.");
                Environment.Exit(1);
            }

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    if (args[0] == "enqueue")
                    {
                        services.AddHostedService<QueueService>();
                    }
                    else if (args[0] == "output")
                    {
                        services.AddHostedService<OutputService>();
                    }
                    else if (args[0] == "benchmark")
                    {
                        services.AddHostedService<BenchmarkService>();
                    }

                    // Create a rules storage client
                    var storage = hostContext.Configuration.GetSection("Storage");
                    var blobStorage = new BlobWorkflowRulesStorage(storage.GetValue<string>("OutputConnectionString"), storage.GetValue<string>("RulesContainerName"), storage.GetValue<string>("RulesBlobName"));
                    services.AddSingleton<IWorkflowRulesStorage>(blobStorage);

                    // Create a producer client that you can use to send events to an event hub
                    var eventHubConnectionString = hostContext.Configuration.GetSection("EventHub").GetValue<string>("InputConnectionString");
                    var producerClient = new EventHubProducerClient(eventHubConnectionString);
                    services.AddSingleton(producerClient);
                });
        }
    }
}
