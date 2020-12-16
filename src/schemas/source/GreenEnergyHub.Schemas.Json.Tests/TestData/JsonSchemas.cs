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

namespace GreenEnergyHub.Schemas.Json.Tests.TestData
{
    internal static class JsonSchemas
    {
        internal const string Schema = "{\"$schema\": \"http://json-schema.org/draft-07/schema\",\"$id\": \"http://example.com/example.json\",\"type\": \"object\",\"title\": \"The root schema\",\"description\": \"The root schema comprises the entire JSON document.\",\"default\": {},\"examples\": [{\"hello\": \"world\"}], \"required\": [\"hello\"], \"properties\": {\"hello\": {\"$id\": \"#/properties/hello\",\"type\": \"string\",\"title\": \"The hello schema\",\"description\": \"An explanation about the purpose of this instance.\",\"default\": \"\",\"examples\": [\"world\"] }}, \"additionalProperties\": false}";

        internal const string ValidJson = "{\"hello\": \"world\"}";
    }
}
