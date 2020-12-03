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
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageTypes;
using GreenEnergyHub.Messaging.MessageTypes.Common;

namespace Energinet.DataHub.Ingestion.Application.Messages
{
    /// <summary>
    /// Represents time series information from metering points.
    /// </summary>
    [HubMessageQueue("CommandQueue")]
    [HubMessage("HubTimeSeriesMessage")]
    public class HubTimeSeriesMessage : IHubMessage
    {
        /// <summary>
        /// The id of the HubTimeSeriesMessage. Should be unique.
        /// </summary>
        public Transaction Transaction { get; set; } = Transaction.NewTransaction();

        /// <summary>
        /// The date this request was made.
        /// </summary>
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The date of the data measurement.
        /// </summary>
        public DateTime ObservationTime { get; set; }

        /// <summary>
        /// The measured meter value.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// The meter ID of the data measurement.
        /// </summary>
        public string MeterId { get; set; } = string.Empty;
    }
}
