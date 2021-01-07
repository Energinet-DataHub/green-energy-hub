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
using GreenEnergyHub.Schemas.Json.Tests.TestData;
using Json.Schema;
using Moq;
using Newtonsoft.Json.Bson;
using Xunit;

namespace GreenEnergyHub.Schemas.Json.Tests
{
    public class GreenEnergyHubSchemaValidatorTests : IClassFixture<ValidationOptionsFixture>
    {
        private const bool DocumentIsValid = true;
        private const bool DocumentIsInvalid = false;

        private readonly ValidationOptionsFixture _validationOptionsFixture;

        public GreenEnergyHubSchemaValidatorTests(ValidationOptionsFixture validationOptionsFixture)
        {
            _validationOptionsFixture = validationOptionsFixture;
        }

        [Theory]
        [InlineData("TestData/ChangeOfSupplierValid.json", SchemaTypes.InitiateChangeSupplier, DocumentIsValid)]
        [InlineData("TestData/ChangeOfSupplierInvalidDate.json", SchemaTypes.InitiateChangeSupplier, DocumentIsInvalid)]
        [InlineData("TestData/ChangeOfSupplierMissingEnergySupplierMridQualifier.json", SchemaTypes.InitiateChangeSupplier, DocumentIsInvalid)]
        [InlineData("TestData/UpdateCustomerMasterDataNoContactAddress.json", SchemaTypes.UpdateCustomerMasterData, DocumentIsValid)]
        [InlineData("TestData/UpdateCustomerMasterDataWithContactAddresses.json", SchemaTypes.UpdateCustomerMasterData, DocumentIsValid)]
        public void AssertThatJsonDocumentIsValid(string jsonDocumentPath, string schemaType, bool expectedResult)
        {
            var document = JsonDocument.Parse(File.ReadAllText(jsonDocumentPath));
            var schema = new SchemaType(schemaType);
            var sut = new GreenEnergyHubSchemaValidator(_validationOptionsFixture.Options, new GreenEnergyHubJsonSchemaProvider());

            var results = sut.ValidateDocument(schema, document);

            Assert.Equal(expectedResult, results);
        }

        [Fact]
        public void Validate_should_use_injected_provider()
        {
            var document = JsonDocument.Parse(JsonSchemas.ValidJson);
            var sut = new Mock<IJsonSchemaProvider>();
            sut.Setup(p => p.GetSchema(It.Is<SchemaType>(p => p.Equals(SchemaType.Default)))).Returns(JsonSchema.FromText(JsonSchemas.Schema));

            var validator = new GreenEnergyHubSchemaValidator(_validationOptionsFixture.Options, sut.Object);
            _ = validator.ValidateDocument(SchemaType.Default, document);

            sut.Verify(u => u.GetSchema(SchemaType.Default), Times.Once);
        }

        [Fact]
        public void Validator_should_throw_an_exception_when_document_is_null()
        {
            var schemaProvider = new Mock<IJsonSchemaProvider>();
            var sut = new GreenEnergyHubSchemaValidator(_validationOptionsFixture.Options, schemaProvider.Object);

            Assert.Throws<ArgumentNullException>(() => _ = sut.ValidateDocument(SchemaType.Default, null));
        }

        [Fact]
        public void Validator_should_thrown_an_exception_when_schema_type_is_not_known()
        {
            var document = JsonDocument.Parse(JsonSchemas.ValidJson);
            var schemaProvider = new Mock<IJsonSchemaProvider>();
            var sut = new GreenEnergyHubSchemaValidator(_validationOptionsFixture.Options, schemaProvider.Object);

            Assert.Throws<ArgumentException>(() => _ = sut.ValidateDocument(SchemaType.Default, document));
        }
    }
}
