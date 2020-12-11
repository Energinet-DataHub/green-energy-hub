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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Energinet.DataHub.Ingestion.Infrastructure.Queue;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageQueue;
using GreenEnergyHub.Messaging.MessageTypes;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Infrastructure.ServiceBus
{
    public class ServiceBusDispatcher : IHubMessageServiceBusDispatcher, IAsyncDisposable
    {
        private static readonly IDictionary<Type, string?> _typeToQueueNameCache = new Dictionary<Type, string?>();
        private readonly ILogger<ServiceBusDispatcher> _logger;
        private readonly ServiceBusClient _client;

        public ServiceBusDispatcher(
            IServiceBusClientFactory clientFactory,
            ILogger<ServiceBusDispatcher> logger)
        {
            if (clientFactory is null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }

            _logger = logger;
            _client = clientFactory.Build();
        }

        public async Task DispatchAsync(IHubMessage actionRequest)
        {
            if (actionRequest == null)
            {
                throw new ArgumentNullException(nameof(actionRequest));
            }

            var topic = ExtractRequestInboundQueueNameFrom(actionRequest);
            var sender = _client.CreateSender(topic);

            var message = TransformActionRequestToServiceBusMessage(actionRequest);
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

        private static ServiceBusMessage TransformActionRequestToServiceBusMessage(IHubMessage actionRequest)
        {
            var serializedActionRequest = JsonSerializer.Serialize(actionRequest);
            var queueMessage = new QueueMessage(serializedActionRequest, actionRequest.GetType().ToString());
            var serializedQueueMessage = JsonSerializer.Serialize(queueMessage);
            var serviceBusMessage = new ServiceBusMessage(serializedQueueMessage);

            return serviceBusMessage;
        }

        private static string ExtractRequestInboundQueueNameFrom(IHubMessage actionRequest)
        {
            var type = actionRequest.GetType();
            string? queueName;

            if (!_typeToQueueNameCache.ContainsKey(type))
            {
                queueName = Attribute.GetCustomAttributes(type)
                    .OfType<HubMessageQueueAttribute>()
                    .SingleOrDefault()?
                    .QueueName;

                _typeToQueueNameCache.Add(type, queueName);
            }
            else
            {
                queueName = _typeToQueueNameCache[type];
            }

            return string.IsNullOrEmpty(queueName)
                ? throw new QueueException($"Could not read queue name from attribute {nameof(HubMessageQueueAttribute)}.")
                : queueName;
        }
    }
}
