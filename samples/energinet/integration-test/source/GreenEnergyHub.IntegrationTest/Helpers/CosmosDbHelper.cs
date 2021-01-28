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
using System.Linq.Expressions;
using System.Threading.Tasks;
using GreenEnergyHub.IntegrationTest.Settings;
using Microsoft.Azure.Documents.Client;
using NodaTime;

namespace GreenEnergyHub.IntegrationTest.Helpers
{
    public class CosmosDbHelper
    {
        private readonly CosmosDBSettings _settings;
        private readonly DocumentClient _client;

        public CosmosDbHelper(CosmosDBSettings settings)
        {
            _settings = settings;
            _client = new DocumentClient(new Uri(settings.EndpointUrl), settings.AuthorizationKey, ConnectionPolicy.Default);
        }

        public async Task<T> QueryItems<T>(Expression<Func<T, bool>> predicate, int maxWaitTimeInSeconds = 300)
        {
            var waitUntil = SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromSeconds(maxWaitTimeInSeconds));
            do
            {
                var option = new FeedOptions { EnableCrossPartitionQuery = true };
                var matchingItem = _client.CreateDocumentQuery<T>(
                            UriFactory.CreateDocumentCollectionUri(_settings.DatabaseName, _settings.CollectionName), option)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();

                if (matchingItem != null)
                {
                    return matchingItem;
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
            while (waitUntil > SystemClock.Instance.GetCurrentInstant());

            throw new Exception($"QueryItems timeout exceeded with {maxWaitTimeInSeconds} seconds");
        }
    }
}
