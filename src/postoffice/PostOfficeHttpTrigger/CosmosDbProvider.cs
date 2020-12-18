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
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Postoffice
{
    public class CosmosDbProvider : IDisposable
    {
        private static string _endpointUri;
        // The primary key for the Azure Cosmos account.
        private static string _primaryKey;

        private readonly string _databaseId = "energinetDocsDB";
        private readonly string _containerId = "energinetDocsContainer";

        // The Cosmos client instance
        private CosmosClient _cosmosClient;
        // The database we will create
        private Database _database;
        // The container we will create.
        private Container _container;
        private ILogger _log;

        // The name of the database and container we will create
        public CosmosDbProvider(ILogger log, string endpoint, string key)
        {
            _endpointUri = endpoint;
            _primaryKey = key;
            _log = log;
            _cosmosClient = new CosmosClient(_endpointUri, _primaryKey);
            _database = _cosmosClient.GetDatabase(_databaseId);
            _container = _database.GetContainer(_containerId);
        }

        public static async Task<CosmosDbProvider> CreateAsync(ILogger log, string endpoint, string key)
        {
            var provider = new CosmosDbProvider(log, endpoint, key);
            provider._log = log;
            provider._cosmosClient = new CosmosClient(_endpointUri, _primaryKey);
            provider._database = await provider._cosmosClient.CreateDatabaseIfNotExistsAsync(provider._databaseId).ConfigureAwait(false);
            provider._container = await provider._database.CreateContainerIfNotExistsAsync(provider._containerId, "/Vendor").ConfigureAwait(false);
            return provider;
        }

        public async Task AddDocAsync(string id, string documentContent)
        {
            var doc = EnerginetDoc.FromString(documentContent);
            doc.Id = id;

            try
            {
                // Read the item to see if it exists
                ItemResponse<EnerginetDoc> docAddResponce = await _container.ReadItemAsync<EnerginetDoc>(doc.Id, new PartitionKey(doc.Vendor)).ConfigureAwait(false);
                _log.LogInformation("Item in database with id: {0} already exists\n", docAddResponce.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                ItemResponse<EnerginetDoc> docAddResponce = await _container.CreateItemAsync<EnerginetDoc>(doc, new PartitionKey(doc.Vendor)).ConfigureAwait(false);
                _log.LogInformation("Created item in database with id: {0} Operation consumed {1} RUs.\n", docAddResponce.Resource.Id, docAddResponce.RequestCharge);
            }
        }

        public async Task DeleteDocAsync(string id)
        {
            EnerginetDoc doc = await GetDocAsync(id).ConfigureAwait(false);
            ItemResponse<EnerginetDoc> deleteAddResponce = await _container.DeleteItemAsync<EnerginetDoc>(doc.Id, new PartitionKey(doc.Vendor)).ConfigureAwait(false);
            _log.LogInformation("Deleted EnerginetDoc [{0},{1}]\n", doc.Vendor, doc.Id);
        }

        public async Task<EnerginetDoc> GetDocAsync(string id)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = '{id}' OFFSET 0 LIMIT 1";

            _log.LogInformation("Running query: {0}\n", sqlQueryText);

            var queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<EnerginetDoc> queryResultSetIterator = _container.GetItemQueryIterator<EnerginetDoc>(queryDefinition);

            var docs = new List<EnerginetDoc>();
            if (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<EnerginetDoc> currentResultSet = await queryResultSetIterator.ReadNextAsync().ConfigureAwait(false);
                if (currentResultSet.Count > 0)
                {
                    return currentResultSet.First<EnerginetDoc>();
                }
            }

            return null;
        }

        public void Dispose()
        {
            _cosmosClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
