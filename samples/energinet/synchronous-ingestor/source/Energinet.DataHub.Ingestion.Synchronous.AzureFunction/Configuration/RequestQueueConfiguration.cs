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
using Energinet.DataHub.Ingestion.Synchronous.Infrastructure.RequestQueue;
using GreenEnergyHub.Messaging.RequestQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Energinet.DataHub.Ingestion.Synchronous.AzureFunction.Configuration
{
    internal static class RequestQueueConfiguration
    {
        internal static IServiceCollection AddRequestQueue(this IServiceCollection services)
        {
            services.AddSingleton<KafkaConfiguration>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new KafkaConfiguration()
                {
                    BoostrapServers = configuration.GetValue<string>("BoostrapServers"),
                    SaslMechanism = configuration.GetValue<string>("SaslMechanism"),
                    SaslUsername = configuration.GetValue<string>("SaslUsername"),
                    SaslPassword = configuration.GetValue<string>("SaslPassword"),
                    SecurityProtocol = configuration.GetValue<string>("SecurityProtocol"),
                    SslCaLocation = configuration.GetValue<string>("SslCaLocation"),
                    MessageTimeoutMs = configuration.GetValue<int>("MessageTimeoutMs"),
                    MessageSendMaxRetries = configuration.GetValue<int>("MessageSendMaxRetries")
                };
            });
            services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
            services.AddSingleton<IHubRequestQueueDispatcher, KafkaDispatcher>();
            return services;
        }
    }
}
