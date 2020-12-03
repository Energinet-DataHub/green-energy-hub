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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageQueue;
using GreenEnergyHub.Messaging.MessageTypes;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Infrastructure.Queue
{
    public class KafkaDispatcher : IHubMessageQueueDispatcher, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IProducer<Null, string> _producer;
        private bool _disposed;

        public KafkaDispatcher(
            IKafkaProducerFactory producerFactory,
            ILogger<KafkaDispatcher> logger)
        {
            if (producerFactory is null)
            {
                throw new ArgumentNullException(nameof(producerFactory));
            }

            _logger = logger;
            _producer = producerFactory.Build();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task DispatchAsync(IHubMessage actionRequest)
        {
            if (actionRequest == null)
            {
                throw new ArgumentNullException(nameof(actionRequest));
            }

            var topic = ExtractRequestInboundQueueNameFrom(actionRequest);

            var producerMessage = CreateProducerMessage(actionRequest);
            var deliveryResult = await _producer.ProduceAsync(topic, producerMessage).ConfigureAwait(false);

            EnsureDelivered(deliveryResult);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _producer.Dispose();
            }

            _disposed = true;
        }

        private static string ExtractRequestInboundQueueNameFrom(IHubMessage actionRequest)
        {
            var inboundQueueName = Attribute.GetCustomAttributes(actionRequest.GetType())
                .OfType<HubMessageQueueAttribute>()
                .Single()
                .QueueName;

            if (string.IsNullOrEmpty(inboundQueueName))
            {
                throw new QueueException($"Could not read inbound queue name from attribute {nameof(HubMessageQueueAttribute)}.");
            }

            return inboundQueueName;
        }

        private static Message<Null, string> CreateProducerMessage(IHubMessage actionRequest)
        {
            var requestType = ExtractRequestTypeNameFrom(actionRequest);
            var serializedActionRequest = JsonSerializer.Serialize(actionRequest);
            var inboundQueueMessage = new QueueMessage(serializedActionRequest, requestType);
            var payload = JsonSerializer.Serialize(inboundQueueMessage);
            return new Message<Null, string>()
            {
                Value = payload,
            };
        }

        private static string ExtractRequestTypeNameFrom(IHubMessage actionRequest)
        {
            var requestTypeName = Attribute.GetCustomAttributes(actionRequest.GetType())
                .OfType<HubMessageAttribute>()
                .Single()
                .Name;

            if (string.IsNullOrEmpty(requestTypeName))
            {
                throw new QueueException($"Could not read request type name from attribute {nameof(HubMessageAttribute)}.");
            }

            return requestTypeName;
        }

        private void EnsureDelivered(DeliveryResult<Null, string> deliveryResult)
        {
            if (deliveryResult.Status != PersistenceStatus.Persisted)
            {
                _logger.LogError("Producer failed to deliver message. {deliveryResult}", deliveryResult);
                throw new QueueException("Failed to dispatch request to inbound queue.");
            }
        }
    }
}
