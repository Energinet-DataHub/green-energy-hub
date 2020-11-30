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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Synchronous.Application;
using Energinet.DataHub.Ingestion.Synchronous.Application.Requests;
using Energinet.DataHub.Ingestion.Synchronous.Infrastructure;
using FluentAssertions;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using GreenEnergyHub.Messaging.RequestQueue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Energinet.DataHub.Ingestion.Synchronous.Tests.Application
{
    public class ChangeOfSupplierTests
    {
        private readonly HubRequestBulkDispatcher _bulkDispatcher;
        private readonly IHubRehydrate _hubRehydrate;

        public ChangeOfSupplierTests()
        {
            var services = new ServiceCollection();
            services.AddGreenEnergyHub(typeof(ChangeOfSupplierRequest).Assembly);
            services.AddScoped<IHubRehydrate>(sp => new JsonMessageDeserializer(NSubstitute.Substitute.For<ILogger<JsonMessageDeserializer>>()));
            services.AddScoped<IHubRequestQueueDispatcher, HubRequestQueueDispatcherStub>();
            var serviceProvider = services.BuildServiceProvider();
            var hubRequestDispatcher = serviceProvider.GetRequiredService<IHubRequestDispatcher>();

            _hubRehydrate = serviceProvider.GetRequiredService<IHubRehydrate>();
            _bulkDispatcher = new HubRequestBulkDispatcher(hubRequestDispatcher);
        }

        [Fact]
        public async Task Should_return_response_with_valid_validation_results()
        {
            var hubRequests = await RehydrateHubRequestsFromFile().ConfigureAwait(false);

            var response = await _bulkDispatcher.DispatchAsync(hubRequests!).ConfigureAwait(false) as CustomHubResponse;

            Assert.DoesNotContain(response?.ValidationResults, x => x.Errors.Any());
        }

        [Fact]
        public async Task Validation_result_must_contain_transactionId_of_request()
        {
            var hubRequests = await RehydrateHubRequestsFromFile().ConfigureAwait(false);
            var expectedTransactionIds = new List<string>(hubRequests.Select(x => x.Transaction.MRid));

            var response = await _bulkDispatcher.DispatchAsync(hubRequests!).ConfigureAwait(false) as CustomHubResponse;

            var returnedTransactionIds = response?.ValidationResults.Select(x => x.TransactionId).ToList();
            Assert.Equal(expectedTransactionIds, returnedTransactionIds);
        }

        private async Task<IEnumerable<IHubRequest>?> RehydrateHubRequestsFromFile()
        {
            await using var inputRequestStream = File.OpenRead("Assets/ChangeSupplierRequestArray.json");
            return await _hubRehydrate.RehydrateCollectionAsync(inputRequestStream, typeof(ChangeOfSupplierRequest)).ConfigureAwait(false);
        }
    }
}
