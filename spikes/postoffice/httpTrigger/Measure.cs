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
using System.Text.Json.Serialization;

namespace Postoffice
{
    public class Measure
    {
        [JsonPropertyName("CorrelationId")]
        public string CorrelationId { get; set; }

        [JsonPropertyName("MarketEvaluationPoint_mRID")]
        public long Meterid { get; set; }

        [JsonPropertyName("MeterReadingPeriodicity")]
        public string MeterReadingPeriodicity { get; set; }

        [JsonPropertyName("Product")]
        public object Product { get; set; }

        [JsonPropertyName("QuantityMeasurementUnit_Name")]
        public object QuantityMeasurementUnit_Name { get; set; }

        [JsonPropertyName("MarketEvaluationPointType")]
        public string MarketEvaluationPointType { get; set; }

        [JsonPropertyName("SettlementMethod")]
        public string SettlementMethod { get; set; }

        [JsonPropertyName("ProcessType")]
        public string MarketDocument_ProcessType { get; set; }

        [JsonPropertyName("RecipientMarketParticipantMarketRole_Type")]
        public string RecipientMarketParticipantMarketRole_Type { get; set; }

        [JsonPropertyName("MarketServiceCategory_Kind")]
        public string MarketServiceCategory_Kind { get; set; }

        [JsonPropertyName("RecipientMarketParticipant_mRID")]
        public long Actorid { get; set; }

        [JsonPropertyName("point")]
        public Point Point { get; set; }

        [JsonPropertyName("Quantity")]
        public string Quantity { get; set; }

        [JsonPropertyName("Quality")]
        public string Quality { get; set; }

        [JsonPropertyName("ObservationTime")]
        public string ObservationTime { get; set; }

        [JsonPropertyName("PeekStatus")]
        public string PeekStatus { get; set; }

        [JsonPropertyName("JobID")]
        public string JobID { get; set; }

        [JsonPropertyName("Reasons")]
        public List<ReasonObject> Reasons { get; set; }
    }
}
