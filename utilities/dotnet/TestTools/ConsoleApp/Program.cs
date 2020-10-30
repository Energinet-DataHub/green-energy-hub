using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
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
                });
        }
    }
}
