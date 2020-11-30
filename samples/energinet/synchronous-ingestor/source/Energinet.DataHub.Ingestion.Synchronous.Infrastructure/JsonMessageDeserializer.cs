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
using GreenEnergyHub.Messaging;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Synchronous.Infrastructure
{
    public class JsonMessageDeserializer : IHubRehydrate
    {
        private readonly ILogger<JsonMessageDeserializer> _logger;

        public JsonMessageDeserializer(
            ILogger<JsonMessageDeserializer> logger)
        {
            _logger = logger;
        }

        public async Task<IHubRequest?> RehydrateAsync(Stream message, Type messageType)
        {
            try
            {
                var request = await JsonSerializer.DeserializeAsync(message, messageType).ConfigureAwait(false);
                return request as IHubRequest;
            }
            catch (JsonException e)
            {
                LogException(e);
            }

            return null;
        }

        public async Task<IEnumerable<IHubRequest>?> RehydrateCollectionAsync(Stream message, Type messageType)
        {
            try
            {
                var genericType = typeof(IEnumerable<>).MakeGenericType(messageType);
                var requests = await JsonSerializer.DeserializeAsync(message, genericType).ConfigureAwait(false);
                return new List<IHubRequest>((IEnumerable<IHubRequest>)requests);
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
