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
