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
using System.Xml;

namespace GreenEnergyHub.DkEbix
{
    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Test if the current node matches the predicate
        /// </summary>
        /// <param name="reader"><see cref="XmlReader"/> instance</param>
        /// <param name="nodeName">Is this the current node-name</param>
        /// <param name="nodeType">Check if it is of this node type</param>
        /// <returns>true - if a match is found</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reader"></paramref> or <paramref name="nodeName"/> is null</exception>
        internal static bool Is(this XmlReader reader, string nodeName, XmlNodeType nodeType = XmlNodeType.Element)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (nodeName == null) throw new ArgumentNullException(nameof(nodeName));

            return
                reader.NodeType == nodeType &&
                reader.Name.Equals(nodeName, StringComparison.InvariantCulture);
        }
    }
}
