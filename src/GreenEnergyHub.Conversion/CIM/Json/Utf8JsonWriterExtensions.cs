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
using System.Text.Json;
using GreenEnergyHub.Conversion.CIM.Components;
using GreenEnergyHub.Conversion.CIM.Values;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    internal static class Utf8JsonWriterExtensions
    {
        internal static void WriteMarketParticipant(
            this Utf8JsonWriter self,
            string propertyName,
            MarketParticipant marketParticipant)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));
            if (marketParticipant == null) throw new ArgumentNullException(nameof(marketParticipant));

            self.WriteStartObject(propertyName);
            self.WriteString(PropertyNames.mRID, marketParticipant.PartyId.Value);
            self.WriteStartObject(PropertyNames.MarketRole);
            self.WriteString(PropertyNames.type, marketParticipant.MarketRole.MarketRoleKind.Value);
            self.WriteEndObject();
            self.WriteEndObject();
        }

        internal static void WriteDateAndOrTime(
            this Utf8JsonWriter self,
            string propertyName,
            DateAndOrTime dateAndOrTime)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            self.WriteStartObject(propertyName);
            if (dateAndOrTime.Date.HasValue) self.WriteString(PropertyNames.date, dateAndOrTime.Date.Value.ToString(Formats.DateFormat, Formats.DefaultCultureInfo));
            if (dateAndOrTime.Time.HasValue) self.WriteString(PropertyNames.time, dateAndOrTime.Time.Value.ToString());

            self.WriteEndObject();
        }

        internal static void WriteDateTime(
            this Utf8JsonWriter self,
            string propertyName,
            DateTime dateTime)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var formatDate = dateTime.ToString(Formats.DateTimeFormat, Formats.DefaultCultureInfo);

            self.WriteString(propertyName, formatDate);
        }
    }
}
