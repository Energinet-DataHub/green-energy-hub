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
using System.Threading.Tasks;
using System.Xml;
using GreenEnergyHub.DkEbix.Parsers;

namespace GreenEnergyHub.DkEbix
{
    /// <summary>
    /// Create a <see cref="RsmParser"/> from an Asynchronous <see cref="XmlReader"/>
    /// </summary>
    public class RsmParserFactory
    {
        private static readonly RsmParserFactory _instance = new ();

        protected internal RsmParserFactory() { }

        /// <summary>
        /// Create the <see cref="RsmParser"/> from the root element and default xml-namespace
        /// </summary>
        /// <param name="reader">Asynchronous <see cref="XmlReader"/></param>
        /// <returns><see cref="RsmParser"/> matching the xml data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null</exception>
        public static Task<RsmParser> CreateParserAsync(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            return _instance.CreateAsync(reader);
        }

        /// <summary>
        /// Create the <see cref="RsmParser"/> from the root element and default xml-namespace
        /// </summary>
        /// <param name="reader">Asynchronous <see cref="XmlReader"/></param>
        /// <returns><see cref="RsmParser"/> matching the xml data</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null</exception>
        public async Task<RsmParser> CreateAsync(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var rootElement = await GetNextElementIdentificationAsync(reader);
            return ResolveRsmParser(rootElement);
        }

        /// <summary>
        /// Lookup a <see cref="RsmParser"/> based on <see cref="ElementIdentification"/>
        /// </summary>
        /// <param name="rootElement">Root element from the xml stream</param>
        /// <returns><see cref="RsmParser"/> for the xml stream</returns>
        /// <exception cref="ResolveRsmParserException">no parser is found</exception>
        protected internal virtual RsmParser ResolveRsmParser(ElementIdentification rootElement)
        {
            if (rootElement.Equals(DocumentFormats.RSM033)) return new Rsm033();

            throw new ResolveRsmParserException("No matching parser found");
        }

        private static async Task<ElementIdentification> GetNextElementIdentificationAsync(XmlReader reader)
        {
            do
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    return new ElementIdentification(reader.Name, reader.NamespaceURI);
                }
            }
            while (await reader.ReadAsync() && !reader.EOF);

            throw new XmlException("No element found");
        }
    }
}
