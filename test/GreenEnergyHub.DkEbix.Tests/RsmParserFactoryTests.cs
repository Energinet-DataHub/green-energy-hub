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
using System.Reflection;
using AutoFixture;
using Xunit;

namespace GreenEnergyHub.DkEbix.Tests
{
    public class RsmParserFactoryTests
    {
        [Fact]
        public void Unknown_document_element_should_throw_an_exception()
        {
            var factory = new RsmParserFactory();
            var fixture = new Fixture();
            var documentElement = fixture.Create<ElementIdentification>();

            Assert.Throws<ResolveRsmParserException>(() => factory.ResolveRsmParser(documentElement));
        }

        [Fact]
        public void All_document_formats_should_resolve_a_parser()
        {
            var documentFormats = typeof(DocumentFormats);
            var factory = new RsmParserFactory();
            var formats = documentFormats.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            foreach (var t in formats)
            {
                var val = t.GetValue(null); // get value of static field
                Assert.NotNull(val);

                var elementIdentification = (ElementIdentification)((val ?? default) ?? throw new InvalidOperationException());

                var parser = factory.ResolveRsmParser(elementIdentification);
                Assert.NotNull(parser);
            }
        }
    }
}
