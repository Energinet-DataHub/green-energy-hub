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
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace GreenEnergyHub.Queues.Kafka
{
    public class KafkaDispatcher : IDisposable, IKafkaDispatcher
    {
        private readonly IProducer<Null, string> _producer;
        private bool _disposed;

        public KafkaDispatcher(
            IKafkaProducerFactory producerFactory)
        {
            if (producerFactory is null)
            {
                throw new ArgumentNullException(nameof(producerFactory));
            }

            _producer = producerFactory.Build();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task DispatchAsync(string message, string topic)
        {
            var producerMessage = CreateProducerMessage(message);
            var deliveryResult = await _producer.ProduceAsync(topic, producerMessage).ConfigureAwait(false);

            EnsureDelivered(deliveryResult);
        }

        public async Task DispatchAsync(MessageEnvelope messageEnvelope, string topic)
        {
            if (messageEnvelope == null)
            {
                throw new ArgumentNullException(nameof(messageEnvelope));
            }

            var producerMessage = CreateProducerMessage(messageEnvelope);
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

        private static Message<Null, string> CreateProducerMessage(MessageEnvelope messageEnvelope)
        {
            var payload = JsonSerializer.Serialize(messageEnvelope);
            return new Message<Null, string>()
            {
                Value = payload,
            };
        }

        private static Message<Null, string> CreateProducerMessage(string message)
        {
            return new Message<Null, string>()
            {
                Value = message,
            };
        }

        private static void EnsureDelivered(DeliveryResult<Null, string> deliveryResult)
        {
            if (deliveryResult.Status != PersistenceStatus.Persisted)
            {
                throw new MessageQueueException("Failed to dispatch request to inbound queue.");
            }
        }
    }
}
