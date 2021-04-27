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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using GreenEnergyHub.Conversion.CIM;
using GreenEnergyHub.Conversion.CIM.Components;
using GreenEnergyHub.Conversion.CIM.Values;

namespace GreenEnergyHub.DkEbix.Parsers
{
    public class Rsm033 : RsmParser
    {
        protected override async Task<MktActivityRecord?> ReadPayloadAsync(XmlReader reader)
        {
            string? identification = null;
            DateTime? occurrence = null;
            List<ChargeType> chargeTypes = new ();

            while (await reader.ReadAsync() && !reader.EOF)
            {
                if (reader.Is("PayloadChargeEvent", XmlNodeType.EndElement))
                {
                    if (identification == null) throw new Exception("Missing identification");
                    if (occurrence == null) throw new Exception("Missing occurrence");

                    var payload = new RequestChangeOfPriceList(
                        identification,
                        DateAndOrTime.FromDateTime(occurrence.Value),
                        DateAndOrTime.FromDateTime(occurrence.Value));

                    payload.ChargeGroup.AddRange(chargeTypes);

                    return payload;
                }

                if (reader.Is("Identification")) identification = await reader.ReadElementContentAsStringAsync();
                if (reader.Is("Occurrence")) occurrence = reader.ReadElementContentAsDateTime();
                if (reader.Is("RelatedChargeChargeInformation")) chargeTypes.Add(await ReadChargeTypeAsync(reader));
            }

            return null;
        }

        private async Task<ChargeType> ReadChargeTypeAsync(XmlReader reader)
        {
            string? chargeOwnerIdentification = null;
            ChargeKind? chargeKind = null;
            string? partyChargeTypeId = null;
            string? description = null;
            string? longDescription = null;
            VatPayerKind? vatPayerKind = null;
            Duration? duration = null;
            List<PricePoint> points = new ();
            bool? taxIndicator = null;
            bool? transparentInvoicing = null;

            while (await reader.ReadAsync() && !reader.EOF)
            {
                if (reader.Is("RelatedChargeChargeInformation", XmlNodeType.EndElement)) break;

                if (reader.Is("ChargeType"))
                {
                    chargeKind = new ChargeKind(await reader.ReadElementContentAsStringAsync());
                }
                else if (reader.Is("PartyChargeTypeID"))
                {
                    partyChargeTypeId = await reader.ReadElementContentAsStringAsync();
                }
                else if (reader.Is("Description"))
                {
                    description = await reader.ReadElementContentAsStringAsync();
                }
                else if (reader.Is("LongDescription"))
                {
                    longDescription = await reader.ReadElementContentAsStringAsync();
                }
                else if (reader.Is("VATClass"))
                {
                    vatPayerKind = VatPayerKind.Parse(await reader.ReadElementContentAsStringAsync());
                }
                else if (reader.Is("TransparentInvoicing"))
                {
                    transparentInvoicing = reader.ReadElementContentAsBoolean();
                }
                else if (reader.Is("TaxIndicator"))
                {
                    taxIndicator = reader.ReadElementContentAsBoolean();
                }
                else if (reader.Is("ChargeTypeOwnerEnergyParty"))
                {
                    if (reader.ReadToDescendant("Identification"))
                    {
                        chargeOwnerIdentification = await reader.ReadElementContentAsStringAsync();
                    }
                }
                else if (reader.Is("ObservationTimeSeriesPeriod"))
                {
                    if (reader.ReadToDescendant("ResolutionDuration"))
                    {
                        duration = new Duration(await reader.ReadElementContentAsStringAsync());
                    }
                }
                else if (reader.Is("IntervalEnergyObservation"))
                {
                    points.Add(await ReadPricePointAsync(reader));
                }
            }

            if (chargeOwnerIdentification == null) throw new Exception("Missing charge owner identification");
            if (duration == null) throw new Exception("Missing duration");
            var period = new SeriesPeriodTimeframe(duration, new TimeFrame(duration));
            period.AddRange(points);

            var chargeType = new ChargeType(
                new PartyId(chargeOwnerIdentification),
                chargeKind,
                mRid: partyChargeTypeId,
                name: description,
                description: longDescription,
                vatPayer: vatPayerKind,
                transparentInvoicing: transparentInvoicing,
                taxIndicator: taxIndicator);

            chargeType.Add(period);

            return chargeType;
        }

        private async Task<PricePoint> ReadPricePointAsync(XmlReader reader)
        {
            int? position = null;
            double? amount = null;

            while (await reader.ReadAsync() && !reader.EOF)
            {
                if (reader.Is("IntervalEnergyObservation", XmlNodeType.EndElement)) break;
                if (reader.Is("Position")) position = reader.ReadElementContentAsInt();
                if (reader.Is("EnergyPrice")) amount = reader.ReadElementContentAsDouble();
            }

            if (position == null) throw new Exception("Missing position");
            if (amount == null) throw new Exception("Missing amount");

            return new PricePoint(position.Value, amount.Value);
        }
    }
}
