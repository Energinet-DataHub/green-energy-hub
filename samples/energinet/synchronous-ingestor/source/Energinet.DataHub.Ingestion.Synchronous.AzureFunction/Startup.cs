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
using Energinet.DataHub.Ingestion.Synchronous.Application.Requests;
using Energinet.DataHub.Ingestion.Synchronous.AzureFunction;
using Energinet.DataHub.Ingestion.Synchronous.AzureFunction.Configuration;
using Energinet.DataHub.Ingestion.Synchronous.Infrastructure;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Energinet.DataHub.Ingestion.Synchronous.AzureFunction
{
    #pragma warning disable CA1812
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register services
            builder.Services.AddScoped<IHubRehydrate, JsonMessageDeserializer>();
            builder.Services.AddGreenEnergyHub(typeof(ChangeOfSupplierRequest).Assembly);
            builder.Services.AddRequestQueue();
        }
    }
    #pragma warning restore CA1812
}
