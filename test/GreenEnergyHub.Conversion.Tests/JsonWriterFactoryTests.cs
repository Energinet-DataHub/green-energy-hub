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

using System.IO;
using System.Linq;
using GreenEnergyHub.Conversion.CIM;
using GreenEnergyHub.Conversion.CIM.Components;
using GreenEnergyHub.Conversion.CIM.Json;
using Xunit;

namespace GreenEnergyHub.Conversion.Tests
{
    public class JsonWriterFactoryTests
    {
        [Fact]
        public void JsonWriterFactory_should_resolve_writer_from_generic_type()
        {
            var writer = JsonWriterFactory.CreateWriter<RequestChangeOfPriceList>(Stream.Null);

            Assert.NotNull(writer);
        }

        [Fact]
        public void JsonWriterFactory_should_resolve_writer_from_payload_type()
        {
            var writer = JsonWriterFactory.CreateWriter(typeof(RequestChangeOfPriceList), Stream.Null);

            Assert.NotNull(writer);
        }

        [Fact]
        public void JsonWriterFactory_should_resolve_all_known_document_types()
        {
            var documentType = typeof(MktActivityRecord);
            var asm = documentType.Assembly;

            var documents = asm.GetTypes()
                .Where(documentType.IsAssignableFrom)
                .Where(cls => cls != documentType);

            Assert.All(documents, type => JsonWriterFactory.CreateWriter(type, Stream.Null));
        }
    }
}
