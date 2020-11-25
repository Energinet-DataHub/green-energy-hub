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
using JetBrains.Annotations;
using Json.Schema;

namespace GreenEnergyHub.JSONSchemaValidator.Validate
{
    /// <summary>
    /// Validation service
    /// </summary>
    public class ValidateService
    {
        private readonly ValidationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateService"/> class.
        /// </summary>
        /// <param name="options">Validation settings</param>
        public ValidateService(ValidationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Validates the document according to the given schema type
        /// </summary>
        /// <param name="type">The type of schema that's about to be validated</param>
        /// <param name="json">The document to be validated</param>
        public ValidationResults ValidateDocument(SchemaType type, [NotNull] JsonDocument json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var schema = SchemaHelper.GetSchema(type);

            if (schema == null)
            {
                throw new ArgumentException("Unsupported schema type", nameof(type));
            }

            return schema.Validate(json.RootElement, _options);
        }
    }
}
