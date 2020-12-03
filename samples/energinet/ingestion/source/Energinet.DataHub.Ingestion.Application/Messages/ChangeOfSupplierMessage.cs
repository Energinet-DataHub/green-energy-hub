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
    /// POCO for a ChangeOfSupplierMessage.
    /// </summary>
    [HubMessageQueue("MessageQueue")]
    [HubMessage("ChangeSupplier")]
    public class ChangeOfSupplierMessage : IHubMessage, IHubMessageHasConsumer, IHubMessageHasEnergySupplier, IHubMessageHasBalanceResponsibleParty, IHubMessageHasStartDate
    {
        /// <summary>
        /// The id of the ChangeOfSupplierMessage message. Should be unique.
        /// </summary>
        public Transaction Transaction { get; set; } = Transaction.NewTransaction();

        /// <summary>
        /// The customer requesting a change of supplier.
        /// </summary>
        public MarketParticipant BalanceResponsibleParty { get; set; } = MarketParticipant.Empty;

        /// <summary>
        /// The new energy supplier
        /// </summary>
        public MarketParticipant EnergySupplier { get; set; } = MarketParticipant.Empty;

        /// <summary>
        /// The customer at the metering point
        /// </summary>
        public MarketParticipant Consumer { get; set; } = MarketParticipant.Empty;

        /// <summary>
        /// Metering point for the change
        /// </summary>
        public MarketEvaluationPoint MarketEvaluationPoint { get; set; } = MarketEvaluationPoint.Empty;

        /// <summary>
        /// Start of occurrence
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date this request was made.
        /// </summary>
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    }
}
