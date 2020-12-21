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
using System.Text.Json.Serialization;
using NodaTime;

namespace GreenEnergyHub.Messaging.MessageTypes.Common
{
    /// <summary>
    /// Market Document Properties
    /// </summary>
    public class MarketDocument
    {
        /// <summary>
        /// Market Document ID.
        /// This corresponds to the "Identification" element within "HeaderEnergyDocument" in RSM-012 time series.
        /// </summary>
        [JsonPropertyName("mRID")]
        public string? MRID { get; set; }

        /// <summary>
        /// What type the received message is eg. E66.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Time of when the time series was created
        /// </summary>
        public Instant? CreatedDateTime { get; set; }

        /// <summary>
        /// The ID and Type of the sender of the time series message
        /// </summary>
        public MarketParticipant? SenderMarketParticipant { get; set; }

        /// <summary>
        /// The ID and Type of the recipient of the time series
        /// </summary>
        public MarketParticipant? RecipientMarketParticipant { get; set; }

        /// <summary>
        /// This is a business reason code. It informs you about the context the RSM-message is used in.
        /// E.g. a RSM-012 time series with code EnergyBusinessProcess = 'D42' is a time series for a flex metering point.
        /// </summary>
        public string? ProcessType { get; set; }

        /// <summary>
        /// Sector area information, e.g. the value E23 is for electricity.
        /// </summary>
        [JsonPropertyName("MarketServiceCategory_Kind")]
        public string? MarketServiceCategoryKind { get; set; }
    }
}
