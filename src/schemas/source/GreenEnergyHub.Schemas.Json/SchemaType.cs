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

using System.Collections.Generic;

namespace GreenEnergyHub.Schemas.Json
{
    /// <summary>
    /// <see cref="SchemaType"/> is a known JsonSchema
    /// </summary>
    public readonly struct SchemaType : System.IEquatable<SchemaType>
    {
        private static readonly HashSet<string> _validSchemaTypes = new HashSet<string>(new[]
        {
            SchemaTypes.InitiateChangeSupplier,
        });

        /// <summary>
        /// Create a new <see cref="SchemaType"/>
        /// </summary>
        /// <param name="schemaName">name of the schema</param>
        public SchemaType(string schemaName)
        {
            Name = schemaName;
        }

        /// <summary>
        /// Default implementation of <see cref="SchemaType"/>
        /// </summary>
        public static SchemaType Default => new SchemaType(string.Empty);

        /// <summary>
        /// Schema name
        /// </summary>
        public string Name { get; }

        public static bool operator ==(SchemaType left, SchemaType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SchemaType left, SchemaType right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Test if schemaName is a known GreenEnergyHub schema
        /// </summary>
        /// <param name="schemaName">Name of the schema</param>
        public static bool IsValid(string schemaName)
        {
            return _validSchemaTypes.Contains(schemaName);
        }

        /// <summary>
        /// Parse a string to a known schema type
        /// </summary>
        /// <param name="schemaName">Name of schema</param>
        /// <param name="schemaType">Constructed <see cref="SchemaType"/></param>
        public static bool TryParse(string schemaName, out SchemaType schemaType)
        {
            schemaType = new SchemaType(schemaName);
            return schemaType.IsValid();
        }

        /// <summary>
        /// Test if the <see cref="SchemaType"/> is valid
        /// </summary>
        public bool IsValid()
        {
            return IsValid(Name);
        }

        /// <summary>
        /// Equality compare
        /// </summary>
        /// <param name="other">Other schema</param>
        /// <returns>true if the values are equal</returns>
        public bool Equals(SchemaType other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is SchemaType other && Equals(other);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
