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
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Application.ChangeOfSupplier;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Queues;
using GreenEnergyHub.Queues.AzureServiceBus;

namespace Energinet.DataHub.Ingestion.Infrastructure.MessageQueue
{
    public class MarketDataMessageQueueDispatcher : IMarketDataMessageQueueDispatcher
    {
        private readonly IServiceBusQueueDispatcher _serviceBusQueueDispatcher;
        private readonly IMessageEnvelopeFactory _messageEnvelopeFactory;
        private readonly string _topic;

        public MarketDataMessageQueueDispatcher(
            IServiceBusQueueDispatcher serviceBusQueueDispatcher,
            IMessageEnvelopeFactory messageEnvelopeFactory,
            string topic)
        {
            _serviceBusQueueDispatcher = serviceBusQueueDispatcher ?? throw new ArgumentNullException(nameof(serviceBusQueueDispatcher));
            _messageEnvelopeFactory = messageEnvelopeFactory ?? throw new ArgumentNullException(nameof(messageEnvelopeFactory));
            _topic = !string.IsNullOrEmpty(topic) ? topic : throw new ArgumentNullException(nameof(topic));
        }

        public Task DispatchAsync(IHubMessage hubMessage)
        {
            if (hubMessage == null)
            {
                throw new ArgumentNullException(nameof(hubMessage));
            }

            return _serviceBusQueueDispatcher.DispatchAsync(_messageEnvelopeFactory.CreateFrom(hubMessage), _topic);
        }
    }
}
