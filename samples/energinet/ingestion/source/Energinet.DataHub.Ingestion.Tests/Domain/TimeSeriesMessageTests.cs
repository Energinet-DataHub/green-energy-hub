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

using System.Text.Json;
using Energinet.DataHub.Ingestion.Domain.TimeSeries;
using GreenEnergyHub.TestHelpers.Traits;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Energinet.DataHub.Ingestion.Tests.Domain
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class TimeSeriesMessageTests
    {
        /// <summary>
        /// IMPORTANT: Do not modify this test without making sure that the output message format
        /// can be consumed by the time series streaming process (Databricks).
        ///
        /// The purpose of this test is to detect if breaking changes has been made to the output format
        /// of time series messages. In that case you must make sure to update the Databricks
        /// streaming job to make sure the change of format doesn't break it.
        /// </summary>
        [Fact]
        public void Output_format_must_comply_with_expected_streaming_input_format()
        {
            // Arrange
            // IMPORTANT: Please do not move this JSON away from this unit test despite that it would improve readability.
            // Reason is that the farther away it is being moving the more likely it becomes that someone changes
            // the format without realizing the consequences.
            var expectedTimeSeriesJson = @"{
  ""mRID"": ""tId2020-12-01T13:16:29.330Z"",
  ""MessageReference"": ""mId2020-12-01T13:16:29.330Z"",
  ""MarketDocument"": {
    ""mRID"": ""hId2020-12-01T13:16:29.330Z"",
    ""Type"": ""E66"",
    ""CreatedDateTime"": ""2020-12-01T13:16:29.33Z"",
    ""SenderMarketParticipant"": {
      ""mRID"": ""8100000000030"",
      ""name"": null,
      ""qualifier"": ""VA"",
      ""Type"": ""MDR""
    },
    ""RecipientMarketParticipant"": {
      ""mRID"": ""5790001330552"",
      ""name"": null,
      ""qualifier"": ""VA"",
      ""Type"": null
    },
    ""ProcessType"": ""D42"",
    ""MarketServiceCategory_Kind"": ""23""
  },
  ""MktActivityRecord_Status"": ""9"",
  ""Product"": ""8716867000030"",
  ""QuantityMeasurementUnit_Name"": ""KWH"",
  ""MarketEvaluationPointType"": ""E17"",
  ""SettlementMethod"": ""D01"",
  ""MarketEvaluationPoint_mRID"": ""571313180000000005"",
  ""CorrelationId"": ""66a28f7f-a8bc-4091-a870-0f54fccc6087"",
  ""Period"": {
    ""Resolution"": ""PT1H"",
    ""TimeInterval"": {
      ""Start"": ""2020-11-20T23:00:00Z"",
      ""End"": ""2020-11-21T23:00:00Z""
    },
    ""Points"": [
      {
        ""Position"": 1,
        ""Quantity"": 1.337,
        ""Quality"": ""E01"",
        ""Time"": ""2020-11-20T23:00:00Z""
      }
    ]
  },
  ""Transaction"": {
    ""mRID"": ""fc13ba2cf048432a951f260ecafb60d4""
  },
  ""RequestDate"": ""2020-12-15T13:15:11.8349163Z""
}";

            var prettyPrintOptions = new JsonSerializerOptions { WriteIndented = true };
            prettyPrintOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var nodaTimeOptions = new JsonSerializerOptions();
            nodaTimeOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var timeSeriesMessage = JsonSerializer.Deserialize<TimeSeriesMessage>(expectedTimeSeriesJson, nodaTimeOptions);

            // Act
            var actual = JsonSerializer.Serialize(timeSeriesMessage, prettyPrintOptions);

            // Assert
            Assert.Equal(expectedTimeSeriesJson.Replace("\r", string.Empty), actual.Replace("\r", string.Empty));
        }
    }
}
