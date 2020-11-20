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
