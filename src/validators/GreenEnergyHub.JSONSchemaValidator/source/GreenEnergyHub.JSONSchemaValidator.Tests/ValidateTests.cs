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
using GreenEnergyHub.JSONSchemaValidator.Validate;
using Json.Schema;
using Xunit;

namespace GreenEnergyHub.JSONSchemaValidator.Tests
{
    public class ValidateTests
    {
        private readonly ValidateService _validateService;

        public ValidateTests()
        {
            var hubSchemas = JsonSchema.FromFile("schemas/cim-definitions.schema.json");
            SchemaRegistry.Global.Register(new Uri("https://github.com/green-energy-hub/schemas"), hubSchemas);

            var options = new ValidationOptions
            {
                OutputFormat = OutputFormat.Verbose, ValidateAs = Draft.Draft7, ValidateFormat = true
            };
            _validateService = new ValidateService(options);
        }

        [Fact]
        public void DocumentShouldBeValid()
        {
            var document = JsonDocument.Parse(File.ReadAllText("TestData/ChangeOfSupplierValid.json"));

            var results = _validateService.ValidateDocument(SchemaType.ChangeOfSupplier, document);

            Assert.True(results.IsValid);
        }

        [Fact]
        public void DocumentDateShouldBeInvalid()
        {
            var document = JsonDocument.Parse(File.ReadAllText("TestData/ChangeOfSupplierInvalidDate.json"));

            var results = _validateService.ValidateDocument(SchemaType.ChangeOfSupplier, document);

            Assert.False(results.IsValid);
        }

        [Fact]
        public void DocumentMissingPropertyShouldBeInvalid()
        {
            var document = JsonDocument.Parse(File.ReadAllText("TestData/ChangeOfSupplierMissingEnergySupplierMridQualifier.json"));

            var results = _validateService.ValidateDocument(SchemaType.ChangeOfSupplier, document);

            Assert.False(results.IsValid);
        }
    }
}
