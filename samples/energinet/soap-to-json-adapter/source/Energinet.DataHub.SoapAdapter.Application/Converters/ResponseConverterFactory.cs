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
using Energinet.DataHub.SoapAdapter.Application.Exceptions;
using Energinet.DataHub.SoapAdapter.Domain;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    public sealed class ResponseConverterFactory : IResponseConverterFactory
    {
        /// <summary>
        /// Factory for response converters
        /// </summary>
        /// <param name="marketDocumentType">MarketDocument type to get the appropriate converter for</param>
        public IResponseConverter GetConverter(string marketDocumentType)
        {
            return marketDocumentType switch
            {
                DocumentTypes.RequestChangeOfSupplier => new SendMessageResponseConverter(),
                DocumentTypes.MeteredDataTimeSeries => new SendMessageResponseConverter(),
                _ => throw new UnknownConverterException(marketDocumentType)
            };
        }
    }
}
