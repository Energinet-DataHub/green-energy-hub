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
using Energinet.DataHub.SoapValidation.Resolvers;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.UnitTests.Resolvers
{
    public class WsdlEmbeddedResourceLocatorStrategyTest
    {
        private const string SchemaSetFolder = "DataHub";
        private const string XsdFile = "SomeFile.xsd";
        private const string AbsoluteUriPrefix = "C:\\SomeWHere\\Does\\Not\\Really\\Matter\\";

        [Fact]
        public void GetResourcePath_WhenXsdFileMatch_Answer()
        {
            // Arrange
            var uri = GetUri();

            var sut = new WsdlEmbeddedResourceLocatorStrategy(SchemaSetFolder, XsdFile);

            // Act
            var result = sut.GetResourcePath(uri);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void GetResourcePath_WhenXsdFileDoesNotMatch_DontAnswer()
        {
            // Arrange
            var uri = GetUri();
            var sut = new WsdlEmbeddedResourceLocatorStrategy(SchemaSetFolder, string.Empty);

            // Act
            var result = sut.GetResourcePath(uri);

            // Assert
            Assert.True(string.IsNullOrWhiteSpace(result));
        }

        private Uri GetUri()
        {
            return new Uri($"{AbsoluteUriPrefix}\\{XsdFile}");
        }
    }
}
