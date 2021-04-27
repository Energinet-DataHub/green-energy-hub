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

namespace GreenEnergyHub.DkEbix
{
    /// <summary>
    /// Object representation of xml element and namespace
    /// </summary>
    public class ElementIdentification : IEquatable<ElementIdentification>
    {
        public ElementIdentification(string elementName, string xmlNamespace)
        {
            ElementName = elementName;
            XmlNamespace = xmlNamespace;
        }

        public string ElementName { get; }

        public string XmlNamespace { get; }

        public bool Equals(ElementIdentification other)
        {
            return ElementName == other.ElementName && XmlNamespace == other.XmlNamespace;
        }

        public override bool Equals(object? obj)
        {
            return obj is ElementIdentification other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ElementName, XmlNamespace);
        }
    }
}
