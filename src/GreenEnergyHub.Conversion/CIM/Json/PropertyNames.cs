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

using System.Diagnostics.CodeAnalysis;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    [SuppressMessage("StyleCop.Analyzers", "SA1303", Justification = "Properties should match that of CIM convention")]
    internal static class PropertyNames
    {
        public const string amount = nameof(amount);
        public const string businessProcessReferenceMktActivityRecord = "businessProcessReference_MktActivityRecord";
        public const string ChargeGroup = nameof(ChargeGroup);
        public const string ChargeType = nameof(ChargeType);
        public const string ChargeTypeOwnerMarketParticipant = "chargeTypeOwner_MarketParticipant";
        public const string createdDateTime = nameof(createdDateTime);
        public const string date = nameof(date);
        public const string description = nameof(description);
        public const string EndDateAndOrTime = "end_DateAndOrTime";
        public const string MarketRole = nameof(MarketRole);
        public const string MktActivityRecord = nameof(MktActivityRecord);
        public const string mRID = nameof(mRID);
        public const string name = nameof(name);
        public const string Point = nameof(Point);
        public const string position = nameof(position);
        public const string price = nameof(price);
        public const string priceTimeFramePeriod = "priceTimeFrame_Period";
        public const string ReceiverMarketParticipant = "Receiver_MarketParticipant";
        public const string resolution = nameof(resolution);
        public const string SenderMarketParticipant = "Sender_MarketParticipant";
        public const string SeriesPeriod = "Series_Period";
        public const string StartDateAndOrTime = "start_DateAndOrTime";
        public const string taxIndicator = nameof(taxIndicator);
        public const string terminationDate = nameof(terminationDate);
        public const string transparentInvoicing = nameof(transparentInvoicing);
        public const string time = nameof(time);
        public const string type = nameof(type);
        public const string VATPayer = nameof(VATPayer);
    }
}
