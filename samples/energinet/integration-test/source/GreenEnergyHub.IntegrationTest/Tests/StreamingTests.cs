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
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GreenEnergyHub.IntegrationTest.Helpers;
using GreenEnergyHub.IntegrationTest.Settings;
using GreenEnergyHub.IntegrationTest.Types;
using GreenEnergyHub.TestHelpers.Traits;
using NodaTime;
using Xunit;

namespace GreenEnergyHub.IntegrationTest.Tests
{
    [Trait(TraitNames.Category, TraitValues.IntegrationTest)]
    public class StreamingTests
    {
        private IntegrationTestSettings _integrationTestSettings;
        private CosmosDbHelper _cosmosDbHelper;
        private EventHubHelper _eventHubHelper;
        private ParquetBlobStorageHelper _parquetBlobStorageHelper;
        private ParquetHelper _parquetHelper;

        [Fact]
        public async Task StreamingSendsValidTimeSeriesToPostOffice()
        {
            // Arrange
            GetSettingsFileAndSetupHelpers();
            var guidForConsumption = Guid.NewGuid().ToString();
            var validJsonConsumption = EmbeddedResourcesHelper.GetContentAsString(@"Assets.valid_time_series_message_for_consumption.json");
            validJsonConsumption = validJsonConsumption.Replace(@"WILL BE REPLACED BY INTEGRATION TEST", $"{guidForConsumption}");

            var guidForExchangeReactive = Guid.NewGuid().ToString();
            var validJsonExchangeReactive = EmbeddedResourcesHelper.GetContentAsString(@"Assets.valid_time_series_message_for_exchange_reactive.json");
            validJsonExchangeReactive = validJsonExchangeReactive.Replace(@"WILL BE REPLACED BY INTEGRATION TEST", $"{guidForExchangeReactive}");

            var consumptionFileDate = new LocalDate(2020, 11, 13);
            var exchangeReactiveFileDate = new LocalDate(2020, 12, 6);

            // Act
            await _eventHubHelper.PlaceItemOnEventHub(validJsonConsumption).ConfigureAwait(false);
            await _eventHubHelper.PlaceItemOnEventHub(validJsonExchangeReactive).ConfigureAwait(false);

            // Assert
            var maxWaitTimeInSeconds = 300;
            var timeSeriesListConsumption = await _parquetHelper
                .FindTimeSeriesInBlobStorage(guidForConsumption, maxWaitTimeInSeconds, consumptionFileDate)
                .ConfigureAwait(false);

            // Find a single time series point
            var point = timeSeriesListConsumption.Single(x => x.Time == Instant.FromUtc(2020, 11, 13, 03, 00));
            point.Should().NotBeNull();
            point.MarketEvaluationPointMRID.Should().Be("571313180000000005");
            point.Time.Should().Be(Instant.FromUtc(2020, 11, 13, 03, 0, 0));
            point.Quantity.Should().Be(5.543M);
            point.CorrelationId.Should().Be(guidForConsumption);
            point.MessageReference.Should().Be("mId2019-12-01T13:16:29.330Z");
            point.MarketDocumentMRID.Should().Be("hId2020-12-01T13:16:29.330Z");
            point.CreatedTime.Should().Be(Instant.FromUtc(2021, 1, 10, 13, 16, 29));
            point.SenderMarketParticipantMRID.Should().Be("8100000000030");
            point.ProcessType.Should().Be("D42");
            point.Quality.Should().Be("A02");
            point.MeterReadingPeriodicity.Should().Be("PT1H");
            point.MeteringMethod.Should().Be("D01");
            point.MeteringGridAreaDomainMRID.Should().Be("800");
            point.ConnectionState.Should().Be("E22");
            point.EnergySupplierMarketParticipantMRID.Should().Be("8100000000108");
            point.BalanceResponsiblePartyMarketParticipantMRID.Should().Be("8100000000207");
            point.InMeteringGridAreaDomainMRID.Should().Be("NULL");
            point.OutMeteringGridAreaDomainMRID.Should().Be("NULL");
            point.ParentDomainMRID.Should().Be("NULL");
            point.ServiceCategoryKind.Should().Be("23");
            point.SettlementMethod.Should().Be("D01");
            point.QuantityMeasurementUnitName.Should().Be("KWH");
            point.Product.Should().Be("8716867000030");
            var timeSeriesListExchange = await _parquetHelper
                .FindTimeSeriesInBlobStorage(guidForExchangeReactive, maxWaitTimeInSeconds, exchangeReactiveFileDate)
                .ConfigureAwait(false);
            var exchangeReactive = timeSeriesListExchange.Single(x => x.Time == Instant.FromUtc(2020, 12, 6, 03, 00));
            exchangeReactive.InMeteringGridAreaDomainMRID.Should().Be("802");
            exchangeReactive.OutMeteringGridAreaDomainMRID.Should().Be("803");
            exchangeReactive.ParentDomainMRID.Should().Be("571313180000000074");
        }

        [Fact]
        public async Task StreamingSendsRejectedTimeSeriesToPostOffice()
        {
            // Arrange
            GetSettingsFileAndSetupHelpers();
            var newCorrelationId = Guid.NewGuid().ToString();
            var invalidTimeSeries = EmbeddedResourcesHelper.GetContentAsString(@"Assets.invalid_time_series_message_too_big_quantity_VR-612.json");
            invalidTimeSeries = invalidTimeSeries.Replace(@"WILL BE REPLACED BY INTEGRATION TEST", $"{newCorrelationId}");

            // Act
            await _eventHubHelper.PlaceItemOnEventHub(invalidTimeSeries).ConfigureAwait(false);

            // Assert
            var queriedTimeSeries = await _cosmosDbHelper
                .QueryItems<InvalidTimeSeries>(t => t.CorrelationId == newCorrelationId)
                .ConfigureAwait(false);
            queriedTimeSeries.Should().NotBeNull();
            queriedTimeSeries.CorrelationId.Should().Be(newCorrelationId);
            queriedTimeSeries.Reasons.First().Reason.Should().Be("VR-612");
            queriedTimeSeries.MessageType.Should().Be("InvalidTimeSeries");
            queriedTimeSeries.ProcessType.Should().Be("D42");
            queriedTimeSeries.RecipientMarketParticipantMarketRole_Type.Should().Be("MDR");
            queriedTimeSeries.RecipientMarketParticipant_mRID.Should().Be("8100000000030");
            queriedTimeSeries.TimeSeries_mRID.Should().Be("tId2020-12-01T13:16:29.330Z");
            queriedTimeSeries.MarketDocument_mRID.Should().Be("hId2020-12-01T13:16:29.330Z");
        }

        private void GetSettingsFileAndSetupHelpers()
        {
            var settingsJsonStream = EmbeddedResourcesHelper.GetContentAsString("local.settings.json");
            var settings = JsonSerializer.Deserialize<IntegrationTestSettings>(settingsJsonStream);
            _integrationTestSettings = settings;
            _cosmosDbHelper = new CosmosDbHelper(settings.CosmosDBSettings);
            _eventHubHelper = new EventHubHelper(settings.EventHubSettings);
            _parquetBlobStorageHelper = new ParquetBlobStorageHelper(settings.BlobStorageSettings);
            _parquetHelper = new ParquetHelper(_parquetBlobStorageHelper);
        }
    }
}
