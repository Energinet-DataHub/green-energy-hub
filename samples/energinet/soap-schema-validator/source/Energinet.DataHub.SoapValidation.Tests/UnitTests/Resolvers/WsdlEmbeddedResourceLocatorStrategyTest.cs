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
