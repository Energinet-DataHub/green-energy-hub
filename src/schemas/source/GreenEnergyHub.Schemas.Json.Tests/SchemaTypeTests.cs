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

using Xunit;

namespace GreenEnergyHub.Schemas.Json.Tests
{
    public class SchemaTypeTests
    {
        [Fact]
        public void Identical_SchemaType_should_be_Equal()
        {
            var schema1 = SchemaType.Default;
            var schema2 = SchemaType.Default;

            Assert.True(schema1 == schema2);
        }

        [Fact]
        public void Different_SchemaType_should_not_be_equal()
        {
            var schema1 = new SchemaType("Schema1");
            var schema2 = new SchemaType("Schema2");

            Assert.True(schema1 != schema2);
        }

        [Fact]
        public void Known_schema_type_should_be_valid()
        {
            var schema = new SchemaType(SchemaTypes.InitiateChangeSupplier);

            Assert.True(schema.IsValid());
            Assert.True(SchemaType.IsValid(SchemaTypes.InitiateChangeSupplier));
        }

        [Fact]
        public void Parse_known_schema_type_should_be_true()
        {
            var expectedSchemaType = new SchemaType(SchemaTypes.InitiateChangeSupplier);
            var isKnown = SchemaType.TryParse(SchemaTypes.InitiateChangeSupplier, out var actualSchemaType);

            Assert.True(isKnown);
            Assert.Equal(expectedSchemaType, actualSchemaType);
        }
    }
}
