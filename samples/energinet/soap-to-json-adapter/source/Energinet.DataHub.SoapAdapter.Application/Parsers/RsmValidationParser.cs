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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapAdapter.Domain;
using Energinet.DataHub.SoapAdapter.Domain.Validation;
using NodaTime;

namespace Energinet.DataHub.SoapAdapter.Application.Parsers
{
    public class RsmValidationParser
    {
        private const string B2BNamespace = "urn:www:datahub:dk:b2b:v01";

        public async ValueTask<Context> ParseAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var validationContext = new Context();
            var position = stream.Position;

            try
            {
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings { Async = true }))
                {
                    await ReadDocumentAsync(reader, validationContext).ConfigureAwait(false);
                }
            }
            finally
            {
                stream.Position = position;
            }

            return validationContext;
        }

        private static async ValueTask<string> ReadIdentificationAsync(XmlReader reader, string ns)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.LocalName == "Identification" && reader.NamespaceURI == ns &&
                    reader.NodeType == XmlNodeType.Element)
                {
                    return await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
            }

            throw new XmlException("Missing xml exception");
        }

        private static bool IsRootElementFound(XmlReader reader, string payloadRootElement, string payloadNamespace)
        {
            return !(reader.NodeType != XmlNodeType.Element
                   && payloadRootElement.Length == 0
                   && payloadNamespace.Length == 0);
        }

        private static bool IsRootElementAssigned(string payloadRootElement, string payloadNamespace)
        {
            return !(payloadRootElement.Length == 0 && payloadNamespace.Length == 0);
        }

        private static async ValueTask ReadHeaderEnergyDocumentAsync(XmlReader reader, Context context, string ns)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.LocalName == "HeaderEnergyDocument" && reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
                else if (reader.Is("Identification", ns))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    context.RsmHeader.Identification = content;
                }
                else if (reader.Is("DocumentType", ns))
                {
                    context.RsmHeader.DocumentType = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("Creation", ns))
                {
                    context.RsmHeader.Creation = Instant.FromDateTimeUtc(reader.ReadElementContentAsDateTime());
                }
                else if (reader.Is("SenderEnergyParty", ns))
                {
                    context.RsmHeader.SenderIdentification = await ReadIdentificationAsync(reader, ns).ConfigureAwait(false);
                }
                else if (reader.Is("RecipientEnergyParty", ns))
                {
                    context.RsmHeader.RecipientIdentification = await ReadIdentificationAsync(reader, ns).ConfigureAwait(false);
                }
            }
        }

        private static async ValueTask ReadProcessEnergyContextAsync(XmlReader reader, Context validationContext, string ns)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is("ProcessEnergyContext", ns, XmlNodeType.EndElement))
                {
                    return;
                }

                if (reader.Is("EnergyBusinessProcess", ns))
                {
                    validationContext.RsmHeader.EnergyBusinessProcess = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("EnergyBusinessProcessRole", ns))
                {
                    validationContext.RsmHeader.EnergyBusinessProcessRole = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("EnergyIndustryClassification", ns))
                {
                    validationContext.RsmHeader.EnergyIndustryClassification = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
            }
        }

        private async ValueTask ReadDocumentAsync(XmlReader reader, Context validationContext)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is("MessageReference", B2BNamespace))
                {
                    validationContext.MessageReference = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("DocumentType", B2BNamespace))
                {
                    validationContext.RsmDocumentType = new DocumentType(await reader.ReadElementContentAsStringAsync().ConfigureAwait(false));
                }
                else if (reader.Is("Payload", B2BNamespace))
                {
                    await ReadPayloadAsync(reader, validationContext).ConfigureAwait(false);
                }
            }
        }

        private async ValueTask ReadPayloadAsync(XmlReader reader, Context validationContext)
        {
            string rootElement = string.Empty;
            string ns = string.Empty;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (!IsRootElementFound(reader, rootElement, ns))
                {
                    continue;
                }

                if (!IsRootElementAssigned(rootElement, ns))
                {
                    rootElement = reader.LocalName;
                    ns = reader.NamespaceURI;
                }
                else if (reader.Is("HeaderEnergyDocument", ns))
                {
                    await ReadHeaderEnergyDocumentAsync(reader, validationContext, ns).ConfigureAwait(false);
                }
                else if (reader.Is("ProcessEnergyContext", ns))
                {
                    await ReadProcessEnergyContextAsync(reader, validationContext, ns).ConfigureAwait(false);
                }

                if (reader.Is("ProcessEnergyContext", ns, XmlNodeType.EndElement))
                {
                    await ReadInnerPayloadsAsync(reader, validationContext, ns).ConfigureAwait(false);
                }
            }
        }

        private async ValueTask ReadInnerPayloadsAsync(XmlReader reader, Context validationContext, string ns)
        {
            string payloadElementName = string.Empty;

            bool PayloadElement(XmlReader internalReader)
            {
                return internalReader.LocalName == payloadElementName
                       && internalReader.NodeType == XmlNodeType.Element
                       && internalReader.NamespaceURI == ns;
            }

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.NodeType != XmlNodeType.Element && payloadElementName.Length == 0)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(payloadElementName))
                {
                    payloadElementName = reader.LocalName;
                }
                else
                {
                    await reader.AdvanceToAsync("Identification", ns).ConfigureAwait(false);

                    var transactionId = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    validationContext.TransactionIds.Add(transactionId);

                    if (!await reader.MoveToNextAsync(PayloadElement).ConfigureAwait(false))
                    {
                        return;
                    }
                }
            }
        }
    }
}
