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
using System.IO;
using GreenEnergyHub.JSONSchemaValidator.Validate;
using Json.Schema;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: FunctionsStartup(typeof(Startup))]

namespace GreenEnergyHub.JSONSchemaValidator.Validate
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register Serilog
            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY") ?? string.Empty;

            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
                .CreateLogger();
            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger));

            // Register services
            string solutionDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            var hubSchemas = JsonSchema.FromFile($"{solutionDir}/GreenEnergyHub.JSONSchemaValidator.Validate/Schemas/cim-definitions.schema.json");
            SchemaRegistry.Global.Register(new Uri("https://github.com/green-energy-hub/schemas"), hubSchemas);

            builder.Services.AddTransient(_ => new ValidationOptions { OutputFormat = OutputFormat.Basic, ValidateFormat = true, ValidateAs = Draft.Draft7 });
            builder.Services.AddSingleton<ValidateService>();
        }
    }
}
