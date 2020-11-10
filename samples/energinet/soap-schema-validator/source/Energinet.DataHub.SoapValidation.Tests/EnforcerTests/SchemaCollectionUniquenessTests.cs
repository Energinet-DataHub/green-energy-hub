using System;
using System.Collections.Generic;
using Energinet.DataHub.SoapValidation.Schemas;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.EnforcerTests
{
    public class SchemaCollectionUniquenessTests
    {
        [Fact]
        public void Ensure_that_schemas_are_unique_in_DataHub_collection()
        {
            // Assemble
            var collection = SchemaCollection.DataHub;

            // Act
            foreach (var definition in collection)
            {
                var schemaDefinition = SchemaCollection.DataHub.Find(definition);

                // Assert
                Assert.NotNull(schemaDefinition);
            }
        }
    }
}
