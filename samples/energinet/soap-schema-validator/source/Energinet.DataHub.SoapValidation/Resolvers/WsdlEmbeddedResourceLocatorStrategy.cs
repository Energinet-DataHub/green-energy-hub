using System;
using System.IO;

namespace Energinet.DataHub.SoapValidation.Resolvers
{
    internal class WsdlEmbeddedResourceLocatorStrategy : IResourceLocatorStrategy
    {
        private string _schemaSetFolder;
        private string _xsdFile;

        internal WsdlEmbeddedResourceLocatorStrategy(string schemaSetFolder, string xsdFile)
        {
            _schemaSetFolder = schemaSetFolder;
            _xsdFile = xsdFile;
        }

        public string GetResourcePath(Uri absoluteUri)
        {
            var resourceLocation = absoluteUri.ToString();
            var file = Path.GetFileName(resourceLocation);
            if (file == _xsdFile)
            {
                return $"Energinet.DataHub.SoapValidation.Schemas.{_schemaSetFolder}.wsdl.{file}";
            }

            return string.Empty;
        }
    }
}
