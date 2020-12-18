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
using Azure.Messaging.ServiceBus;

namespace GreenEnergyHub.Queues.AzureServiceBus
{
    public class ServiceBusQueueDispatcher : IAsyncDisposable, IServiceBusQueueDispatcher
    {
        private readonly ServiceBusClient _client;

        public ServiceBusQueueDispatcher(
            IServiceBusClientFactory clientFactory)
        {
            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }

            _client = clientFactory.Build();
        }

        public async Task DispatchAsync(MessageEnvelope messageEnvelope, string topic)
        {
            if (messageEnvelope == null)
            {
                throw new ArgumentNullException(nameof(messageEnvelope));
            }

            var sender = _client.CreateSender(topic);
            var message = TransformQueueMessageToServiceBusMessage(messageEnvelope);
            await sender.SendMessageAsync(message).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (!(_client is null))
            {
                await _client.DisposeAsync().ConfigureAwait(false);
            }

            GC.SuppressFinalize(this);
        }

        private static ServiceBusMessage TransformQueueMessageToServiceBusMessage(MessageEnvelope messageEnvelope)
        {
            var serializedQueueMessage = JsonSerializer.Serialize(messageEnvelope);
            var serviceBusMessage = new ServiceBusMessage(serializedQueueMessage);

            return serviceBusMessage;
        }
    }
}
