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
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.Domain.Validation;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    public class ChangeSupplierConverter : RsmConverter
    {
        private const string B2BNamespace = "un:unece:260:data:EEM-DK_RequestChangeOfSupplier:v3";

        protected override async ValueTask ConvertPayloadAsync(XmlReader reader, RsmHeader header, Utf8JsonWriter writer)
        {
            writer.WriteStartArray();

            while (await reader.ReadAsync())
            {
                if (reader.Is("DK_RequestChangeOfSupplier", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
                else if (reader.Is("PayloadMPEvent", B2BNamespace))
                {
                    await ProcessPayloadMpEventAsync(reader, header, writer);
                }
            }

            writer.WriteEndArray();
        }

        private static async ValueTask<string> GetChildIdentificationAsync(XmlReader reader)
        {
            if (reader.ReadToFollowing("Identification", B2BNamespace))
            {
                return await reader.ReadElementContentAsStringAsync();
            }

            return string.Empty;
        }

        private static async ValueTask ProcessConsumerPartyAsync(
            XmlReader reader,
            RsmHeader header,
            Utf8JsonWriter writer)
        {
            string? cpr = null;
            string? cvr = null;
            string? qualifier = null;
            string? name = null;

            while (await reader.ReadAsync())
            {
                if (reader.Is("ConsumerConsumerParty", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }

                if (reader.Is("CPR", B2BNamespace) && cpr == null)
                {
                    cpr = await reader.ReadElementContentAsStringAsync();
                    qualifier = "AAR";
                }
                else if (reader.Is("CVR", B2BNamespace) && cvr == null)
                {
                    cvr = await reader.ReadElementContentAsStringAsync();
                    qualifier = "VA";
                }
                else if (reader.Is("Name", B2BNamespace))
                {
                    name = await reader.ReadElementContentAsStringAsync();
                }
            }

            if ((cvr != null && cpr != null) || (cvr == null && cpr == null))
            {
                throw new Exception("Inconsistency in CVR or CPR");
            }

            writer.WriteStartObject("Consumer");
            writer.WriteStartObject("mRID");
            writer.WriteString("value", cvr ?? cpr);
            writer.WriteString("qualifier", qualifier);
            writer.WriteEndObject();

            if (name != null)
            {
                writer.WriteString("name", name);
            }

            writer.WriteEndObject();
        }

        private async ValueTask ProcessPayloadMpEventAsync(XmlReader reader, RsmHeader header, Utf8JsonWriter writer)
        {
            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                if (reader.Is("PayloadMPEvent", B2BNamespace))
                {
                    writer.WriteStartObject();
                }
                else if (reader.Is("PayloadMPEvent", B2BNamespace, XmlNodeType.EndElement))
                {
                    writer.WriteEndObject();
                }
                else if (reader.Is("Identification", B2BNamespace))
                {
                    writer.WriteStartObject("Transaction");
                    writer.WriteString("mRID", await reader.ReadElementContentAsStringAsync());
                    writer.WriteEndObject();
                }
                else if (reader.Is("StartOfOccurrence", B2BNamespace))
                {
                    if (DateTimeOffset.TryParse(await reader.ReadElementContentAsStringAsync(), out var startDate))
                    {
                        writer.WriteString("StartDate", startDate);
                    }
                }
                else if (reader.Is("MeteringPointDomainLocation", B2BNamespace))
                {
                    writer.WriteStartObject("MarketEvaluationPoint");
                    writer.WriteString("mRID", await GetChildIdentificationAsync(reader));
                    writer.WriteEndObject();
                }
                else if (reader.Is("BalanceSupplierEnergyParty", B2BNamespace))
                {
                    writer.WriteStartObject("EnergySupplier");
                    writer.WriteString("mRID", await GetChildIdentificationAsync(reader));
                    writer.WriteEndObject();
                }
                else if (reader.Is("BalanceResponsiblePartyEnergyParty", B2BNamespace))
                {
                    writer.WriteStartObject("BalanceResponsibleParty");
                    writer.WriteString("mRID", await GetChildIdentificationAsync(reader));
                    writer.WriteEndObject();
                }
                else if (reader.Is("ConsumerConsumerParty", B2BNamespace))
                {
                    await ProcessConsumerPartyAsync(reader, header, writer);
                }
            }
            while (await reader.ReadAsync());
        }
    }
}
