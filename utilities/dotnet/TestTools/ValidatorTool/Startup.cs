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
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using ValidatorTool.RuleEngines.MSRE;
using ValidatorTool.RuleEngines.NRules;

[assembly: FunctionsStartup(typeof(ValidatorTool.Startup))]

namespace ValidatorTool
{
    /// <summary>
    /// Read from the config file, and inject the corresponding implementations
    /// into the IoC container
    /// </summary>
    public class Startup : FunctionsStartup
    {
        private const string RuleEngineTypeAppSetting = "RuleEngineType";
        private const string AzureStorageConnectionSetting = "AZURE_STORAGE_CONNECTION_STRING";
        private const string ContainerNameSetting = "RulesContainerName";
        private const string BlobNameSetting = "RulesBlobName";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var connectionString = Environment.GetEnvironmentVariable(AzureStorageConnectionSetting);
            var containerName = Environment.GetEnvironmentVariable(ContainerNameSetting);
            var blobName = Environment.GetEnvironmentVariable(BlobNameSetting);

            var blobStorage = new BlobWorkflowRulesStorage(connectionString, containerName, blobName);
            builder.Services.AddSingleton<IWorkflowRulesStorage>(blobStorage);

            var ruleEngineType = Environment.GetEnvironmentVariable(RuleEngineTypeAppSetting);
            builder.Services.AddSingleton<IRuleEngine>((s) =>
            {
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
