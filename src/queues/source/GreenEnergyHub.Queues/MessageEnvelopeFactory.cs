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
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageTypes;

namespace GreenEnergyHub.Queues
{
    public class MessageEnvelopeFactory : IMessageEnvelopeFactory
    {
        private readonly IJsonSerializer _jsonSerializer;

        public MessageEnvelopeFactory(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public MessageEnvelope CreateFrom(IHubMessage hubMessage)
        {
            if (hubMessage is null)
            {
                throw new ArgumentNullException(nameof(hubMessage));
            }

            var requestType = ExtractMessageTypeNameFrom(hubMessage);
            var serializedActionRequest = _jsonSerializer.Serialize<object>(hubMessage);
            return new MessageEnvelope(serializedActionRequest, requestType);
        }

        private static string ExtractMessageTypeNameFrom(IHubMessage hubMessage)
        {
            var messageTypeName
                = Attribute.GetCustomAttributes(hubMessage.GetType())
                    .OfType<HubMessageAttribute>()
                    .Single()
                    .Name;

            if (string.IsNullOrEmpty(messageTypeName))
            {
                throw new MessageQueueException($"Could not read request type name from attribute {nameof(HubMessageAttribute)}.");
            }

            return messageTypeName;
        }
    }
}
