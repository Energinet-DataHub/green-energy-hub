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
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using GreenEnergyHub.Conversion.CIM;
using GreenEnergyHub.Conversion.CIM.Components;
using GreenEnergyHub.DkEbix;
using GreenEnergyHub.DkEbix.Parsers;
using Xunit;

namespace GreenEnergyHub.DkEbix.Tests
{
    public class ConversionServiceTests
    {
        [Fact]
        public void A_known_parser_should_return_true()
        {
            var rsm033Parser = typeof(Rsm033);

            Assert.True(ConversionService.ContainsParser(rsm033Parser));
        }

        [Fact]
        public void A_unknown_parser_should_return_false()
        {
            var unknownParser = typeof(UnknownParser);

            Assert.False(ConversionService.ContainsParser(unknownParser));
        }

        [Fact]
        public void All_known_parsers_should_return_true()
        {
            var rsmParser = typeof(RsmParser);
            var asm = rsmParser.Assembly;

            var allImplementations = asm.GetTypes()
                .Where(rsmParser.IsAssignableFrom) // and
                .Where(cls => !cls.IsAbstract);

            Assert.All(allImplementations, type => Assert.True(ConversionService.ContainsParser(type)));
        }

        [Fact]
        public async Task Valid_xml_document_should_parse_without_errors()
        {
            var sourceXml = File.OpenRead("Assets/RSM033.xml");
            var target = new MemoryStream();

            var service = new ConversionService();
            await service.ConvertStreamAsync(sourceXml, target);

            Assert.True(target.Position > 0);
        }

        [Fact]
        public async Task Null_checks_on_input_variables()
        {
            var conversionService = new ConversionService();

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                conversionService.ConvertStreamAsync(null!, Stream.Null));

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                conversionService.ConvertStreamAsync(Stream.Null, null!));
        }

        private class UnknownParser : RsmParser
        {
            protected override Task<MktActivityRecord?> ReadPayloadAsync(XmlReader reader)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
