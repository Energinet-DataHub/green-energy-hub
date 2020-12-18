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
using Energinet.DataHub.Ingestion.Infrastructure.MessageQueue;
using GreenEnergyHub.Queues;
using GreenEnergyHub.Queues.Kafka;
using GreenEnergyHub.Queues.Kafka.Integration.ServiceCollection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Energinet.DataHub.Ingestion.Synchronous.AzureFunction.Configuration
{
    internal static class MessageQueueConfiguration
    {
        internal static IServiceCollection AddMessageQueue(this IServiceCollection services)
        {
            services.AddSingleton<KafkaConfiguration>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new KafkaConfiguration()
                {
                    BoostrapServers = configuration.GetValue<string>("REQUEST_QUEUE_URL"),
                    SaslMechanism = configuration.GetValue<string>("KAFKA_SASL_MECHANISM"),
                    SaslUsername = configuration.GetValue<string>("KAFKA_USERNAME"),
                    SaslPassword = configuration.GetValue<string>("REQUEST_QUEUE_CONNECTION_STRING"),
                    SecurityProtocol = configuration.GetValue<string>("KAFKA_SECURITY_PROTOCOL"),
                    SslCaLocation = Environment.ExpandEnvironmentVariables(configuration.GetValue<string>("KAFKA_SSL_CA_LOCATION")),
                    MessageTimeoutMs = configuration.GetValue<int>("KAFKA_MESSAGE_TIMEOUT_MS"),
                    MessageSendMaxRetries = configuration.GetValue<int>("KAFKA_MESSAGE_SEND_MAX_RETRIES"),
                };
            });
            services.AddKafkaQueueDispatcher();

            services.AddSingleton<IHubMessageQueueDispatcher>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                string messageQueueTopic = configuration.GetValue<string>("REQUEST_QUEUE_TOPIC");
                return new CommonMessageQueueDispatcher(
                    sp.GetRequiredService<IKafkaDispatcher>(),
                    sp.GetRequiredService<IMessageEnvelopeFactory>(),
                    messageQueueTopic);
            });
            return services;
        }
    }
}
