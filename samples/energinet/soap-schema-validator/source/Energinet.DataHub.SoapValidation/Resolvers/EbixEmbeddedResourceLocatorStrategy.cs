using System;
using System.IO;

namespace Energinet.DataHub.SoapValidation.Resolvers
{
    internal class EbixEmbeddedResourceLocatorStrategy : IResourceLocatorStrategy
    {
        private readonly string _folder;
        private readonly string _documentType;

        public EbixEmbeddedResourceLocatorStrategy(string folder, string documentType)
        {
            _folder = folder;
            _documentType = documentType;
        }

        public string GetResourcePath(Uri absoluteUri)
        {
            var resourceLocation = absoluteUri.ToString();
            var file = Path.GetFileName(resourceLocation);
            if (resourceLocation.Contains("/core/", StringComparison.InvariantCultureIgnoreCase))
            {
               return $"Energinet.DataHub.SoapValidation.Schemas.{_folder}.core.{file}";
            }

            if (resourceLocation.Contains("generic", StringComparison.InvariantCultureIgnoreCase))
            {
                return $"Energinet.DataHub.SoapValidation.Schemas.{_folder}.generic.{file}";
            }

            return $"Energinet.DataHub.SoapValidation.Schemas.{_folder}.document.{_documentType}.{file}";
        }
    }
}