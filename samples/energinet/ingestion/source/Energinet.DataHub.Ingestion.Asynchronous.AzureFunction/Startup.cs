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
using Energinet.DataHub.Ingestion.Application.TimeSeries;
using Energinet.DataHub.Ingestion.Asynchronous.AzureFunction;
using Energinet.DataHub.Ingestion.Asynchronous.AzureFunction.Configuration;
using Energinet.DataHub.Ingestion.Domain.TimeSeries;
using Energinet.DataHub.Ingestion.Infrastructure;
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Energinet.DataHub.Ingestion.Asynchronous.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Register services
            builder.Services.AddScoped<IHubRehydrator, JsonMessageDeserializer>();
            builder.Services.AddGreenEnergyHub(typeof(TimeSeriesCommandHandler).Assembly);
            builder.Services.AddGreenEnergyHub(typeof(TimeSeriesMessage).Assembly);
            builder.Services.AddMarketDataMessageQueue();
            builder.Services.AddTimeSeriesMessageQueue();
            builder.Services.AddScoped<IHubCommandMediator, HubCommandMediator>();
            builder.Services.AddSingleton<IJsonSerializer, JsonSerializer>();
        }
    }
}
