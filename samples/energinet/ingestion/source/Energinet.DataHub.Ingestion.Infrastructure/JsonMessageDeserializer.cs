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
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Infrastructure
{
    public class JsonMessageDeserializer : IHubRehydrator
    {
        private readonly ILogger<JsonMessageDeserializer> _logger;
        private readonly IJsonSerializer _jsonSerializer;

        public JsonMessageDeserializer(
            ILogger<JsonMessageDeserializer> logger,
            IJsonSerializer jsonSerializer)
        {
            _logger = logger;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<IHubMessage?> RehydrateAsync(Stream message, Type messageType)
        {
            try
            {
                var request = await _jsonSerializer.DeserializeAsync(message, messageType).ConfigureAwait(false);
                return request as IHubMessage;
            }
            catch (JsonException e)
            {
                LogException(e);
            }

            return null;
        }

        public async Task<IEnumerable<IHubMessage>?> RehydrateCollectionAsync(Stream message, Type messageType)
        {
            try
            {
                var genericType = typeof(IEnumerable<>).MakeGenericType(messageType);
                var messages = await _jsonSerializer.DeserializeAsync(message, genericType).ConfigureAwait(false);
                return new List<IHubMessage>((IEnumerable<IHubMessage>)messages);
            }
            catch (JsonException e)
            {
                LogException(e);
            }

            return null;
        }

        private void LogException(Exception exception)
        {
            _logger.LogError(exception, "Unable to rehydrate message");
        }
    }
}
