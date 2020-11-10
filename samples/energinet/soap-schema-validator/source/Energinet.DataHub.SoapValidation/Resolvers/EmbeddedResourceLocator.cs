using System;
using System.IO;
using System.Xml;

namespace Energinet.DataHub.SoapValidation.Resolvers
{
    internal class EmbeddedResourceLocator : XmlUrlResolver
    {
        private readonly IResourceLocatorStrategy _locatorStrategy;

        public EmbeddedResourceLocator()
            : this(new DefaultEmbeddedResourceLocatorStrategy()) { }

        public EmbeddedResourceLocator(IResourceLocatorStrategy strategy)
        {
            _locatorStrategy = strategy;
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var stream = GetResourceStream(absoluteUri);
            return stream ?? throw new InvalidOperationException("Unable to locate requested resource");
        }

        private Stream? GetResourceStream(Uri absoluteUri)
        {
            var resourcePath = _locatorStrategy.GetResourcePath(absoluteUri);

            var asm = GetType().Assembly;
            return asm.GetManifestResourceStream(resourcePath);
        }
    }
}