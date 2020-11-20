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
using System.Reflection;
using System.Threading.Tasks;
using Energinet.DataHub.SoapValidation.Dtos;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.UnitTests
{
    public class SchemaFinderTests
    {
        private const string TestFileFolder = "Energinet.DataHub.SoapValidation.Tests.TestFiles";
        private const string Rsm012Folder = "Rsm012";

        [Fact]
        public async Task TopLevelFind_should_locate_schema()
        {
            await using var stream = GetInputStream("ValidHourly.xml", Rsm012Folder); // valid with soap
            var sut = new XmlSchemaValidator();

            var result = await sut.ValidateStreamAsync(stream);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task TopLevelFind_should_not_locate_schema()
        {
            await using var stream = GetInputStream("ValidHourlyButNoSOAP.xml", Rsm012Folder);
            var sut = new XmlSchemaValidator();
            var result = await sut.ValidateStreamAsync(stream);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(RejectionReason.SchemasUnavailable, result.RejectionReason);
        }

        [Theory]
        [InlineData("ValidHourly.xml", false, RejectionReason.None)]
        [InlineData("ValidHourlyButNoSOAP.xml", false, RejectionReason.SchemasUnavailable)]
        [InlineData("ValidHourlyButNoSOAP.xml", true, RejectionReason.None)]
        public async Task LocateSchemas(string filename, bool traverse, RejectionReason reason)
        {
            await using var stream = GetInputStream(filename, Rsm012Folder);
            var sut = new XmlSchemaValidator();

            var result = await sut.ValidateStreamAsync(stream, traverse);

            Assert.NotNull(result);
            Assert.Equal(reason, result.RejectionReason);
        }

        private Stream GetInputStream(string fileName, string messageTypeFolder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{TestFileFolder}.{messageTypeFolder}.{fileName}";
            var result = assembly.GetManifestResourceStream(resourceName);
            if (result == null)
            {
                throw new NotImplementedException($"The filename {fileName} has not been added as an embedded resource to the project");
            }

            return result;
        }
    }
}
