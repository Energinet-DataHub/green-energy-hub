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
using System.Linq;
using GreenEnergyHub.JSONSchemaValidator.Validate;
using Xunit;

namespace GreenEnergyHub.JSONSchemaValidator.Tests
{
    public class SchemaHelperTests
    {
        [Fact]
        public void SchemaHelper_should_contain_schemas()
        {
            string[] schemas = SchemaHelper.Schemas.ToArray();

            Assert.NotEmpty(schemas);
        }

        [Fact]
        public void SchemaHelper_should_resolve_schema()
        {
            string[] schemas = SchemaHelper.Schemas.ToArray();

            var jsonSchema = SchemaHelper.GetSchema(schemas[0]);

            Assert.NotNull(jsonSchema);
        }

        [Fact]
        public void SchemaHelper_should_resolve_all_schema_types()
        {
            var schemaTypes = Enum.GetValues(typeof(SchemaType));

            foreach (var schemaTypeValue in schemaTypes)
            {
                if (schemaTypeValue == null)
                {
                    continue;
                }

                var schemaType = (SchemaType)schemaTypeValue;

                var jsonSchema = SchemaHelper.GetSchema(schemaType);

                Assert.NotNull(jsonSchema);
            }
        }
    }
}
