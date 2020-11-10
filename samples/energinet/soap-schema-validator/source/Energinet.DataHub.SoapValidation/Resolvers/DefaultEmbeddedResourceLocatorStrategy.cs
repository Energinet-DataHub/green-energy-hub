using System;
using System.IO;

namespace Energinet.DataHub.SoapValidation.Resolvers
{
    internal class DefaultEmbeddedResourceLocatorStrategy : IResourceLocatorStrategy
    {
        public string GetResourcePath(Uri absoluteUri)
        {
            var file = Path.GetFileName(absoluteUri.ToString());
            return $"Energinet.DataHub.SoapValidation.Schemas.{file}";
        }
    }
}