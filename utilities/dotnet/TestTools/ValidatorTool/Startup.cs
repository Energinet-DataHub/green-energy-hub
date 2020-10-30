using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using ValidatorTool.RuleEngines.NRules;
using ValidatorTool.RuleEngines.MSRE;

[assembly: FunctionsStartup(typeof(ValidatorTool.Startup))]

namespace ValidatorTool
{
    /// <summary>
    /// Read from the config file, and inject the corresponding implementations
    /// into the IoC container
    /// </summary>
    public class Startup : FunctionsStartup
    {
        const string ruleEngineTypeAppSetting = "RuleEngineType";
        const string azureStorageConnectionSetting = "AZURE_STORAGE_CONNECTION_STRING";
        const string containerNameSetting = "RulesContainerName";
        const string blobNameSetting = "RulesBlobName";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable(azureStorageConnectionSetting);
            var containerName = Environment.GetEnvironmentVariable(containerNameSetting);
            var blobName = Environment.GetEnvironmentVariable(blobNameSetting);

            var blobStorage = new BlobWorkflowRulesStorage(connectionString, containerName, blobName);
            builder.Services.AddSingleton<IWorkflowRulesStorage>(blobStorage);

            var ruleEngineType = Environment.GetEnvironmentVariable(ruleEngineTypeAppSetting);
            builder.Services.AddSingleton<IRuleEngine>((s) => {
                switch (ruleEngineType.ToLower())
                {
                    case "nrules":
                        return new NRulesEngine();
                    case "rulesengine":
                        return new MSREEngine(blobStorage);
                    case "fluent":
                        return new FluentValidationEngine();
                    default:
                        throw new InvalidOperationException($"Invalid engine type {ruleEngineType} specified");
                }
            });
          }
    }
}