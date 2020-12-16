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
using System.Text.Json;
using Json.Schema;

namespace GreenEnergyHub.Schemas.Json
{
    /// <summary>
    /// Json schema validator for GreenEnergyHub
    /// </summary>
    public sealed class GreenEnergyHubSchemaValidator
    {
        private readonly ValidationOptions _validationOptions;
        private readonly IJsonSchemaProvider _jsonSchemaProvider;

        public GreenEnergyHubSchemaValidator(ValidationOptions validationOptions, IJsonSchemaProvider jsonSchemaProvider)
        {
            _validationOptions = validationOptions;
            _jsonSchemaProvider = jsonSchemaProvider;
        }

        /// <summary>
        /// Validate a <see cref="JsonDocument"/> for a given <see cref="SchemaType"/>
        /// </summary>
        /// <param name="schemaType"><see cref="SchemaType"/> to match</param>
        /// <param name="document"><see cref="JsonDocument"/> to validate</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="schemaType"/> is not known to the <see cref="IJsonSchemaProvider"/> service</exception>
        public bool ValidateDocument(SchemaType schemaType, JsonDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var schema = _jsonSchemaProvider.GetSchema(schemaType);
            if (schema == null)
            {
                throw new ArgumentException("Unsupported schema type", nameof(schemaType));
            }

            var result = schema.Validate(document.RootElement, _validationOptions);
            return result?.IsValid ?? false;
        }
    }
}
