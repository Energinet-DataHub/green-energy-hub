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
