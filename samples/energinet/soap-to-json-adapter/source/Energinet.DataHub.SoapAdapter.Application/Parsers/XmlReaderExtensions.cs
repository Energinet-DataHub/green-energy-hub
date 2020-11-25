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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml;

namespace Energinet.DataHub.SoapAdapter.Application.Parsers
{
    public static class XmlReaderExtensions
    {
        public static async Task<bool> MoveToNextElementByNameAsync(
            this XmlReader reader,
            string elementName,
            string xmlNamespace)
        {
            bool Predicate(XmlReader internalReader)
            {
                return internalReader.LocalName == elementName
                       && internalReader.NodeType == XmlNodeType.Element
                       && internalReader.NamespaceURI == xmlNamespace;
            }

            return await reader.MoveToNextAsync(Predicate).ConfigureAwait(false);
        }

        public static async Task<bool> MoveToNextAsync(this XmlReader reader, Func<XmlReader, bool> predicate)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (predicate(reader))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Is(this XmlReader reader, string localName, string ns, XmlNodeType xmlNodeType = XmlNodeType.Element)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (localName == null)
            {
                throw new ArgumentNullException(nameof(localName));
            }

            if (ns == null)
            {
                throw new ArgumentNullException(nameof(ns));
            }

            return reader.LocalName.Equals(localName) && reader.NamespaceURI.Equals(ns) &&
                   reader.NodeType == xmlNodeType;
        }

        public static async ValueTask<XmlReader> AdvanceToAsync(this XmlReader reader, string localName, string ns)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (localName == null)
            {
                throw new ArgumentNullException(nameof(localName));
            }

            if (ns == null)
            {
                throw new ArgumentNullException(nameof(ns));
            }

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.LocalName == localName && reader.NamespaceURI == ns &&
                    reader.NodeType == XmlNodeType.Element)
                {
                    return reader;
                }
            }

            throw new XmlException("Xml node not found");
        }

        public static async ValueTask AdvanceToAsync(this XmlReader reader, XmlNodeType nodeType)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (!Enum.IsDefined(typeof(XmlNodeType), nodeType))
            {
                throw new InvalidEnumArgumentException(nameof(nodeType), (int)nodeType, typeof(XmlNodeType));
            }

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.NodeType != nodeType)
                {
                    continue;
                }

                break;
            }
        }
    }
}
