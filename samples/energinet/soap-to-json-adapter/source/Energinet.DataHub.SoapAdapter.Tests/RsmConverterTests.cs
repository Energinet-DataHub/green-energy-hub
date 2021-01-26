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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application.Converters;
using FluentAssertions;
using GreenEnergyHub.TestHelpers.Traits;
using Xunit;

namespace Energinet.DataHub.SoapAdapter.Tests
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class RsmConverterTests
    {
        [Fact]
        public async Task Validate_valid_rsm001_converted_output()
        {
            // Arrange
            await using var ms = new MemoryStream();
            await using var fs = File.OpenRead("Assets/Rsm001CPR.xml");

            var sut = new ChangeSupplierConverter();

            // Act
            await sut.ConvertAsync(fs, ms).ConfigureAwait(false);

            // Assert
            var json = await GetAsStringAsync(ms).ConfigureAwait(false);

            Assert.True(json.Contains("SessId-0.58783300-1582196206", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("*****************", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("6311125", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("11111111", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Validate_valid_rsm012_converted_output()
        {
            // Arrange
            await using var ms = new MemoryStream();
            await using var fs = File.OpenRead("Assets/RSM-012_ValidHourly.xml");

            var sut = new TimeSeriesConverter();

            // Act
            await sut.ConvertAsync(fs, ms).ConfigureAwait(false);

            // Assert
            var json = await GetAsStringAsync(ms).ConfigureAwait(false);

            Assert.True(json.Contains("\"mRID\":\"TransactionID-0003\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"MessageReference\":\"MessageID-0003\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"MarketDocument\":{", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"mRID\":\"Identification-0003\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"Type\":\"E66\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"CreatedDateTime\":\"2020-06-29T12:38:55Z\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"SenderMarketParticipant\":{\"mRID\":\"8100000000030\",\"qualifier\":\"VA\",\"name\":null,\"Type\":\"MDR\"}", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"RecipientMarketParticipant\":{\"mRID\":\"5790001330552\",\"qualifier\":\"VA\",\"name\":null,\"Type\":null}", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"ProcessType\":\"D42\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"MarketServiceCategory_Kind\":\"23\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"MktActivityRecord_Status\":\"9\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"Product\":\"8716867000030\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"QuantityMeasurementUnit_Name\":\"KWH\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"SettlementMethod\":\"D01\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"MarketEvaluationPoint_mRID\":\"578032999778756222\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"CorrelationId\":\"Unknown\"", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"Period\":{\"Resolution\":\"PT1H\",\"TimeInterval\":{\"Start\":\"2020-06-27T22:00:00Z\",\"End\":\"2020-06-28T22:00:00Z\"}", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("\"Points\":[{\"Position\":1,\"Quantity\":1.337,\"Quality\":\"E01\",\"Time\":\"2020-06-27T22:00:00Z", StringComparison.OrdinalIgnoreCase));
            Regex.Matches(json, "\"mRID\":").Count.Should().Be(5);
            Regex.Matches(json, "\"Position\":").Count.Should().Be(24);
            Regex.Matches(json, "\"Quantity\":").Count.Should().Be(24);
            Regex.Matches(json, "\"Quality\":").Count.Should().Be(24);
            Regex.Matches(json, "\"Time\":").Count.Should().Be(24);
        }

        [Obsolete("When integration tests are implemented these checks are better safeguarded there")]
        [Fact]
        public async Task ConvertAsync_ValidateTheOutputJson_FileIsAsExpected()
        {
            // Arrange
            await using var ms = new MemoryStream();
            await using var xmlFileStream = File.OpenRead("Assets/RSM-012_ValidHourly.xml");
            await using var expectedJsonFileStream = File.OpenRead("Assets/SinglePurposeJsonUsedToValidateSoapAdapterNotAllowedInOtherTests.json");

            var sut = new TimeSeriesConverter();

            // Act
            await sut.ConvertAsync(xmlFileStream, ms).ConfigureAwait(false);

            // Assert
            var result = await GetAsStringAsync(ms).ConfigureAwait(false);
            var expectedJson = await GetAsStringAsync(expectedJsonFileStream).ConfigureAwait(false);

            // Remove RequestDate value check as it should be set in Entry Point instead of Soap Adapter, kept to ensure pipeline is working
            var trimmedResult = result.Substring(0, result.IndexOf("RequestDate", StringComparison.InvariantCulture));
            var trimmedExpectedJson = expectedJson.Substring(0, expectedJson.IndexOf("RequestDate", StringComparison.InvariantCulture));

            trimmedResult.Should().BeEquivalentTo(trimmedExpectedJson);
        }

        [Fact]
        public async Task Validate_valid_bundled_rsm012_converted_output()
        {
            // Arrange
            await using var ms = new MemoryStream();
            await using var fs = File.OpenRead("Assets/RSM-012_ValidBundledTimeSeries_15min_and_1hour.xml");

            var sut = new TimeSeriesConverter();

            // Act
            await sut.ConvertAsync(fs, ms).ConfigureAwait(false);

            // Assert
            var json = await GetAsStringAsync(ms).ConfigureAwait(false);

            // System.IO.File.WriteAllText(@"c:\temp\converted" + Guid.NewGuid(), json);
            Assert.True(json.Contains("578030100000089755", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("578030100000090898", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("PT15M", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("PT1H", StringComparison.OrdinalIgnoreCase));
        }

        private static async Task<string> GetAsStringAsync(Stream stream)
        {
            stream.Position = 0;
            using var sr = new StreamReader(stream);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
