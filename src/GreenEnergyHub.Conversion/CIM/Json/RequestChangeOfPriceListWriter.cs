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
using System.Text.Json;
using GreenEnergyHub.Conversion.CIM.Components;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    public class RequestChangeOfPriceListWriter : JsonPayloadWriter<RequestChangeOfPriceList>
    {
        protected override void WritePayload(Utf8JsonWriter writer, RequestChangeOfPriceList payload)
        {
            writer.WriteStartObject();

            writer.WriteString(PropertyNames.mRID, payload.MRid);
            writer.WriteDateAndOrTime(PropertyNames.createdDateTime, payload.CreatedDateTime);
            writer.WriteDateAndOrTime(PropertyNames.StartDateAndOrTime, payload.StartDateAndOrTime);
            if (payload.EndDateAndOrTime != null)
            {
                writer.WriteDateAndOrTime(PropertyNames.EndDateAndOrTime, payload.EndDateAndOrTime);
            }

            if (payload.BusinessProcessReferenceMktActivityRecord != null)
            {
                writer.WriteStartObject(PropertyNames.businessProcessReferenceMktActivityRecord);
                writer.WriteString(PropertyNames.mRID, payload.BusinessProcessReferenceMktActivityRecord.Value);
                writer.WriteEndObject();
            }

            WriteChargeGroup(writer, payload.ChargeGroup);

            writer.WriteEndObject();
        }

        private static void WriteChargeGroup(Utf8JsonWriter writer, ChargeGroup chargeGroup)
        {
            writer.WriteStartObject(PropertyNames.ChargeGroup);
            writer.WriteStartArray(PropertyNames.ChargeType);

            foreach (var chargeType in chargeGroup.ChargeTypes)
            {
                writer.WriteStartObject();

                writer.WriteStartObject(PropertyNames.ChargeTypeOwnerMarketParticipant);
                writer.WriteString(PropertyNames.mRID, chargeType.PartyId.Value);
                writer.WriteEndObject(); // end ChargeTypeOwnerMarketParticipant

                if (chargeType.ChargeKind != null) writer.WriteString(PropertyNames.type, chargeType.ChargeKind.Value);
                if (chargeType.Description != null) writer.WriteString(PropertyNames.description, chargeType.Description);
                if (chargeType.TransparentInvoicing.HasValue) writer.WriteBoolean(PropertyNames.transparentInvoicing, chargeType.TransparentInvoicing.Value);
                if (chargeType.TaxIndicator.HasValue) writer.WriteBoolean(PropertyNames.taxIndicator, chargeType.TaxIndicator.Value);
                if (chargeType.TerminationDate.HasValue) writer.WriteString(PropertyNames.terminationDate, chargeType.TerminationDate.Value);
                if (chargeType.MRid != null) writer.WriteString(PropertyNames.mRID, chargeType.MRid);
                if (chargeType.VatPayer != null) writer.WriteString(PropertyNames.VATPayer, chargeType.VatPayer.Value);
                if (chargeType.Name != null) writer.WriteString(PropertyNames.name, chargeType.Name);

                WriteSeriesPeriod(writer, chargeType.SeriesPeriods);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static void WriteSeriesPeriod(
            Utf8JsonWriter writer,
            IReadOnlyCollection<SeriesPeriodTimeframe> periodTimeframes)
        {
            writer.WriteStartArray(PropertyNames.SeriesPeriod);

            foreach (var timeframe in periodTimeframes)
            {
                writer.WriteStartObject();
                writer.WriteString(PropertyNames.resolution, timeframe.Resolution.Value);
                writer.WriteStartObject(PropertyNames.priceTimeFramePeriod);
                writer.WriteString(PropertyNames.resolution, timeframe.PriceTimeFrame.Resolution.Value);
                writer.WriteEndObject(); // end priceTimeFrame_Period

                WritePricePoint(writer, timeframe.Points);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WritePricePoint(Utf8JsonWriter writer, IReadOnlyCollection<PricePoint> points)
        {
            writer.WriteStartArray(PropertyNames.Point);
            foreach (var point in points)
            {
                writer.WriteStartObject();
                writer.WriteNumber(PropertyNames.position, point.Position);
                writer.WriteStartObject(PropertyNames.price);
                writer.WriteNumber(PropertyNames.amount, point.Amount);
                writer.WriteEndObject();
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
