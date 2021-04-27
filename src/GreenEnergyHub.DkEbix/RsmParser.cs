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

namespace GreenEnergyHub.DkEbix
{
    /// <summary>
    /// Parse xml data to CIM objects
    /// </summary>
    public abstract class RsmParser
    {
        public async IAsyncEnumerable<MktActivityRecord> ReadPayloadsAsync(XmlReader reader)
        {
            MktActivityRecord? payload;
            do
            {
                payload = await ReadPayloadAsync(reader);
                if (payload != null) yield return payload;
            }
            while (payload != null);
        }

        /// <summary>
        /// Read a <see cref="MarketDocument"/>
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> from where data is read</param>
        /// <returns><see cref="MarketDocument"/> from the xml stream</returns>
        /// <exception cref="MandatoryDataException">if a mandatory field is not found</exception>
        public async Task<MarketDocument> ReadMarketDocumentAsync(XmlReader reader)
        {
            string? identification = null;
            string? documentType = null;
            DateTime? creation = null;
            string? senderIdentification = null;
            string? receiverIdentification = null;
            string? process = null;

            while (await reader.ReadAsync() && !reader.EOF)
            {
                if (reader.Is("ProcessEnergyContext", XmlNodeType.EndElement)) break;
                if (reader.Is("Identification")) identification = await reader.ReadElementContentAsStringAsync();
                if (reader.Is("DocumentType")) documentType = await reader.ReadElementContentAsStringAsync();
                if (reader.Is("Creation")) creation = reader.ReadElementContentAsDateTime();
                if (reader.Is("SenderEnergyParty"))
                {
                    if (reader.ReadToDescendant("Identification"))
                    {
                        senderIdentification = await reader.ReadElementContentAsStringAsync();
                    }
                }

                if (reader.Is("RecipientEnergyParty"))
                {
                    if (reader.ReadToDescendant("Identification"))
                    {
                        receiverIdentification = await reader.ReadElementContentAsStringAsync();
                    }
                }

                if (reader.Is("EnergyBusinessProcess")) process = await reader.ReadElementContentAsStringAsync();
            }

            if (identification == null) throw new MandatoryDataException("Missing identification", "Identification");
            if (documentType == null) throw new MandatoryDataException("Missing document type", "DocumentType");
            if (creation == null) throw new MandatoryDataException("Missing creation date", "Creation");
            if (senderIdentification == null) throw new MandatoryDataException("Sender identification is missing", "SenderIdentification");
            if (receiverIdentification == null) throw new MandatoryDataException("Receiver identification is missing", "ReceiverIdentification");
            if (process == null) throw new MandatoryDataException("Process is missing", "EnergyBusinessProcess");

            return new MarketDocument(
                identification,
                new MessageKind(documentType),
                creation.Value,
                new Process(new ProcessKind(process)),
                new MarketParticipant(new PartyId(senderIdentification), new MarketRole(new MarketRoleKind("Sender"))),
                new MarketParticipant(new PartyId(receiverIdentification), new MarketRole(new MarketRoleKind("Receiver"))));
        }

        protected abstract Task<MktActivityRecord?> ReadPayloadAsync(XmlReader reader);
    }
}
