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

namespace Energinet.DataHub.SoapValidation.Dtos
{
    internal class DocumentDefinition
    {
        public DocumentDefinition(string rootElement, string targetNamespace)
        {
            RootElement = rootElement;
            Namespace = targetNamespace;
            Identifier = CreateIdentifier(rootElement, targetNamespace);
        }

        public string RootElement { get; }

        public string Namespace { get; }

        public string Identifier
        {
            get;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is DocumentDefinition other)
            {
                return Equals(other);
            }

            return obj.GetType() == GetType() && Equals((DocumentDefinition)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RootElement, Namespace);
        }

        protected static string CreateIdentifier(string rootElement, string targetNamespace)
            => $"{rootElement}#{targetNamespace}";

        private bool Equals(DocumentDefinition other)
        {
            return RootElement == other.RootElement && Namespace == other.Namespace;
        }
    }
}
