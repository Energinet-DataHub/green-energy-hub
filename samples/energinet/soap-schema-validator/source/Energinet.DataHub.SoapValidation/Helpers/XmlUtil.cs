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
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapValidation.Dtos;

namespace Energinet.DataHub.SoapValidation.Helpers
{
    internal static class XmlUtil
    {
        /// <summary>
        /// Retrieve a <see cref="DocumentDefinition"/> from a xml-stream
        /// </summary>
        /// <param name="xmlStream">xml-stream to search</param>
        /// <returns><see cref="DocumentDefinition"/> for the xml-stream</returns>
        /// <exception cref="XmlException">If no definition if found due to invalid xml</exception>
        public static async Task<DocumentDefinition> GetDocumentIdentificationAsync(Stream xmlStream)
        {
            if (xmlStream == null)
            {
                throw new ArgumentNullException(nameof(xmlStream));
            }

            var position = xmlStream.Position;

            var settings = new XmlReaderSettings
            {
                Async = true,
            };

            try
            {
                using var reader = XmlReader.Create(xmlStream, settings);

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        return new DocumentDefinition(reader.LocalName, reader.NamespaceURI);
                    }
                }

                throw new XmlException("Unable to locate root element and/or namespace");
            }
            finally
            {
                xmlStream.Position = position;
            }
        }
    }
}
