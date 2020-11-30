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
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.Domain.Validation;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    public abstract class RsmConverter : IRequestConverter
    {
        private const string B2BNamespace = "urn:www:datahub:dk:b2b:v01";

        public async ValueTask ConvertAsync(Stream input, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            using (var reader = XmlReader.Create(input, new XmlReaderSettings { Async = true }))
            {
                var header = new RsmHeader();
                var writer = new Utf8JsonWriter(output);

                try
                {
                    reader.ReadToFollowing("Payload", B2BNamespace);

                    await ParseRsmHeaderAsync(reader, header).ConfigureAwait(false);

                    await ConvertPayloadAsync(reader, header, writer).ConfigureAwait(false);

                    await writer.FlushAsync().ConfigureAwait(false);
                }
                finally
                {
                    output.Position = 0;
                    await writer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }

        protected abstract ValueTask ConvertPayloadAsync(XmlReader reader, RsmHeader header, Utf8JsonWriter writer);

        private static bool RootElementNotFound(XmlReader reader, string payloadRootElement, string payloadNamespace)
        {
            return reader.NodeType != XmlNodeType.Element
                   && payloadRootElement.Length == 0
                   && payloadNamespace.Length == 0;
        }

        private static bool IfRootElementIsNotAssigned(string payloadRootElement, string payloadNamespace)
        {
            return payloadRootElement.Length == 0 && payloadNamespace.Length == 0;
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

        private static async ValueTask ParseRsmHeaderAsync(XmlReader reader, RsmHeader header)
        {
            string rootElement = string.Empty;
            string ns = string.Empty;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (RootElementNotFound(reader, rootElement, ns))
                {
                    continue;
                }

                if (IfRootElementIsNotAssigned(rootElement, ns))
                {
                    rootElement = reader.LocalName;
                    ns = reader.NamespaceURI;
                }
                else if (reader.Is("HeaderEnergyDocument", ns))
                {
                    await ReadHeaderEnergyDocumentAsync(reader, header, ns).ConfigureAwait(false);
                }
                else if (reader.Is("ProcessEnergyContext", ns))
                {
                    await ReadProcessEnergyContextAsync(reader, header, ns).ConfigureAwait(false);
                }

                if (reader.Is("ProcessEnergyContext", ns, XmlNodeType.EndElement))
                {
                    break;
                }
            }
        }

        private static async ValueTask ReadHeaderEnergyDocumentAsync(XmlReader reader, RsmHeader rsmHeader, string ns)
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
                    rsmHeader.Identification = content;
                }
                else if (reader.Is("DocumentType", ns))
                {
                    rsmHeader.DocumentType = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("Creation", ns))
                {
                    rsmHeader.Creation = reader.ReadElementContentAsDateTime();
                }
                else if (reader.Is("SenderEnergyParty", ns))
                {
                    rsmHeader.SenderIdentification = await ReadIdentificationAsync(reader, ns).ConfigureAwait(false);
                }
                else if (reader.Is("RecipientEnergyParty", ns))
                {
                    rsmHeader.RecipientIdentification = await ReadIdentificationAsync(reader, ns).ConfigureAwait(false);
                }
            }
        }

        private static async ValueTask ReadProcessEnergyContextAsync(XmlReader reader, RsmHeader rsmHeader, string ns)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is("ProcessEnergyContext", ns, XmlNodeType.EndElement))
                {
                    return;
                }

                if (reader.Is("EnergyBusinessProcess", ns))
                {
                    rsmHeader.EnergyBusinessProcess = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("EnergyBusinessProcessRole", ns))
                {
                    rsmHeader.EnergyBusinessProcessRole = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
                else if (reader.Is("EnergyIndustryClassification", ns))
                {
                   rsmHeader.EnergyIndustryClassification = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
