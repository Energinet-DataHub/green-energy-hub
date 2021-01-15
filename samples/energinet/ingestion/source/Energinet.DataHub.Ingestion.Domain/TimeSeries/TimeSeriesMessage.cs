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

using System.Text.Json.Serialization;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageTypes;
using GreenEnergyHub.Messaging.MessageTypes.Common;
using NodaTime;
using Period = Energinet.DataHub.Ingestion.Domain.Messages.Period;

namespace Energinet.DataHub.Ingestion.Domain.TimeSeries
{
    /// <summary>
    /// Contains the time series payload
    /// </summary>
    [HubMessage("TimeSeriesMessage")]
    public class TimeSeriesMessage : IHubMessage
    {
        /// <summary>
        /// ID of this transaction, aka TransactionID and TimeseriesID.
        /// </summary>
        [JsonPropertyName("mRID")]
        public string? MRID { get; set; }

        /// <summary>
        /// The message reference of the B2B Message Header.
        /// </summary>
        public string? MessageReference { get; set; }

        /// <summary>
        /// Market information
        /// </summary>
        public MarketDocument? MarketDocument { get; set; }

        /// <summary>
        /// A code that in combination with EnergyBusinessProcess describes the action to be done, e.g. cancellation, update etc.
        /// </summary>
        [JsonPropertyName("MktActivityRecord_Status")]
        public string? MktActivityRecordStatus { get; set; }

        /// <summary>
        /// Used for identifying the product, e.g. active energy, reactive energy etc.
        /// </summary>
        public string? Product { get; set; }

        /// <summary>
        /// Identifies the measurement unit, e.g. kWh, mWh, kVARh etc.
        /// </summary>
        [JsonPropertyName("QuantityMeasurementUnit_Name")]
        public string? QuantityMeasurementUnitName { get; set; }

        /// <summary>
        /// The type of metering point, e.g. a consumption or production metering point.
        /// </summary>
        public string? MarketEvaluationPointType { get; set; }

        /// <summary>
        /// Identifies how a consumption metering point is settled, e.g. flex settled.
        /// </summary>
        public string? SettlementMethod { get; set; }

        /// <summary>
        /// ID of the metering point
        /// </summary>
        [JsonPropertyName("MarketEvaluationPoint_mRID")]
        public string? MarketEvaluationPointMRID { get; set; }

        /// <summary>
        /// ID shared by the time series points originating from the same original input.
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Time series points
        /// </summary>
        public Period? Period { get; set; }

        /// <summary>
        /// The id of the TimeSeriesMessage. Should be unique.
        /// </summary>
        public Transaction Transaction { get; set; } = Transaction.NewTransaction();

        /// <summary>
        /// The date this request was made.
        /// </summary>
        public Instant RequestDate { get; set; } = SystemClock.Instance.GetCurrentInstant();
    }
}
