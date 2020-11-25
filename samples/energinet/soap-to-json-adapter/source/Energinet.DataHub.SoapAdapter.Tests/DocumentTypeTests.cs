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

using System.Linq;
using System.Reflection;
using Energinet.DataHub.SoapAdapter.Domain;
using Energinet.DataHub.SoapAdapter.Tests.Fixtures;
using Xunit;

// ReSharper disable CA1707
namespace Energinet.DataHub.SoapAdapter.Tests
{
    public class DocumentTypeTests
    {
        [Fact]
        public void All_known_document_types_should_be_valid()
        {
            var documentTypes = typeof(DocumentTypes);
            var members = documentTypes
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field => field.GetValue(null))
                .Cast<string>();

            Assert.All(members, field =>
            {
                var documentType = new DocumentType(field);
                Assert.True(documentType.IsValid());
            });
        }

        [Theory]
        [ClassData(typeof(DocumentTypesFixture))]
        public void All_soap_document_types_should_be_valid(string soapDocumentType)
        {
            var documentType = new DocumentType(soapDocumentType);

            Assert.True(documentType.IsValid());
        }
    }
}
