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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Application;
using Energinet.DataHub.Ingestion.Application.ChangeOfSupplier;
using Energinet.DataHub.Ingestion.Infrastructure;
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using GreenEnergyHub.Queues;
using GreenEnergyHub.TestHelpers.Traits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Energinet.DataHub.Ingestion.Tests.Application
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class ChangeOfSupplierTests
    {
        private readonly HubRequestBulkMediator _bulkMediator;
        private readonly IHubRehydrator _hubRehydrator;

        public ChangeOfSupplierTests()
        {
            var services = new ServiceCollection();
            services.AddGreenEnergyHub(typeof(ChangeOfSupplierMessage).Assembly);
            services.AddScoped<IHubRehydrator>(
                sp => new JsonMessageDeserializer(
                    Substitute.For<ILogger<JsonMessageDeserializer>>(),
                    new JsonSerializer()));
            services.AddScoped<IHubMessageQueueDispatcher, HubMessageQueueDispatcherStub>();
            var serviceProvider = services.BuildServiceProvider();
            var hubRequestMediator = serviceProvider.GetRequiredService<IHubRequestMediator>();

            _hubRehydrator = serviceProvider.GetRequiredService<IHubRehydrator>();
            _bulkMediator = new HubRequestBulkMediator(hubRequestMediator);
        }

        [Fact]
        public async Task Should_return_response_with_valid_validation_results()
        {
            var hubRequests = await RehydrateHubRequestsFromFile().ConfigureAwait(false);

            var response = await _bulkMediator.DispatchAsync(hubRequests!).ConfigureAwait(false) as CustomHubResponse;

            Assert.DoesNotContain(response?.ValidationResults, x => x.Errors.Any());
        }

        [Fact]
        public async Task Validation_result_must_contain_mrid_of_request()
        {
            var hubRequests = await RehydrateHubRequestsFromFile().ConfigureAwait(false);
            var expectedTransactionIds = new List<string>(hubRequests.Select(x => x.Transaction.MRID));

            var response = await _bulkMediator.DispatchAsync(hubRequests!).ConfigureAwait(false) as CustomHubResponse;

            var returnedTransactionIds = response?.ValidationResults.Select(x => x.Transaction.MRID).ToList();
            Assert.Equal(expectedTransactionIds, returnedTransactionIds);
        }

        private async Task<IEnumerable<IHubMessage>?> RehydrateHubRequestsFromFile()
        {
            await using var inputRequestStream = File.OpenRead("Assets/ChangeSupplierRequestArray.json");
            return await _hubRehydrator.RehydrateCollectionAsync(inputRequestStream, typeof(ChangeOfSupplierMessage))
                .ConfigureAwait(false);
        }
    }
}
