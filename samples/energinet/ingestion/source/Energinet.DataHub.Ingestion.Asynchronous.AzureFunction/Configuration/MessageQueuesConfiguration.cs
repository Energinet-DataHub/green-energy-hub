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
using Energinet.DataHub.Ingestion.Application.ChangeOfSupplier;
using Energinet.DataHub.Ingestion.Application.TimeSeries;
using Energinet.DataHub.Ingestion.Infrastructure.MessageQueue;
using GreenEnergyHub.Queues;
using GreenEnergyHub.Queues.AzureServiceBus;
using GreenEnergyHub.Queues.AzureServiceBus.Integration.ServiceCollection;
using GreenEnergyHub.Queues.Kafka;
using GreenEnergyHub.Queues.Kafka.Integration.ServiceCollection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Energinet.DataHub.Ingestion.Asynchronous.AzureFunction.Configuration
{
    internal static class MessageQueuesConfiguration
    {
        internal static void AddTimeSeriesMessageQueue(this IServiceCollection services)
        {
            services.AddSingleton<KafkaConfiguration>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new KafkaConfiguration()
                {
                    BoostrapServers = configuration.GetValue<string>("TIMESERIES_QUEUE_URL"),
                    SaslMechanism = configuration.GetValue<string>("KAFKA_SASL_MECHANISM"),
                    SaslUsername = configuration.GetValue<string>("KAFKA_USERNAME"),
                    SaslPassword = configuration.GetValue<string>("TIMESERIES_QUEUE_CONNECTION_STRING"),
                    SecurityProtocol = configuration.GetValue<string>("KAFKA_SECURITY_PROTOCOL"),
                    SslCaLocation = Environment.ExpandEnvironmentVariables(configuration.GetValue<string>("KAFKA_SSL_CA_LOCATION")),
                    MessageTimeoutMs = configuration.GetValue<int>("KAFKA_MESSAGE_TIMEOUT_MS"),
                    MessageSendMaxRetries = configuration.GetValue<int>("KAFKA_MESSAGE_SEND_MAX_RETRIES"),
                };
            });
            services.AddKafkaQueueDispatcher();
            services.AddSingleton<ITimeSeriesMessageQueueDispatcher>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                string messageQueueTopic = configuration.GetValue<string>("TIMESERIES_QUEUE_TOPIC");
                return new TimeSeriesMessageQueueDispatcher(
                    sp.GetRequiredService<IKafkaDispatcher>(),
                    sp.GetRequiredService<IMessageEnvelopeFactory>(),
                    messageQueueTopic);
            });
        }

        internal static IServiceCollection AddMarketDataMessageQueue(this IServiceCollection services)
        {
            services.AddSingleton<ServiceBusConfiguration>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                return new ServiceBusConfiguration()
                {
                    ConnectionString = configuration.GetValue<string>("MARKET_DATA_CONNECTION_STRING"),
                };
            });
            services.AddServiceBusQueueDispatcher();
            services.AddSingleton<IMarketDataMessageQueueDispatcher>(sp =>
            {
                var configuration = sp.GetService<IConfiguration>();
                string marketDataQueueName = configuration.GetValue<string>("MARKET_DATA_QUEUE");
                return new MarketDataMessageQueueDispatcher(
                    sp.GetRequiredService<IServiceBusQueueDispatcher>(),
                    sp.GetRequiredService<IMessageEnvelopeFactory>(),
                    marketDataQueueName);
            });
            return services;
        }
    }
}
