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

// TODO: Merge this file with the MessageQueueConfiguration.cs in the
// Synchronous Azure Function.
using Energinet.DataHub.Ingestion.Infrastructure.Queue;
using Energinet.DataHub.Ingestion.Infrastructure.ServiceBus;
using GreenEnergyHub.Messaging.MessageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Energinet.DataHub.Ingestion.Asynchronous.AzureFunction.Configuration
{
    internal static class MessageServiceBusConfiguration
    {
        internal static IServiceCollection AddMessageServiceBus(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusConfiguration>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new ServiceBusConfiguration()
                {
                    ConnectionString = configuration.GetValue<string>("ServiceBusConnectionString"),
                };
            });
            services.AddSingleton<IServiceBusClientFactory, ServiceBusClientFactory>();
            services.AddSingleton<IHubMessageServiceBusDispatcher, ServiceBusDispatcher>();
            return services;
        }
    }
}
