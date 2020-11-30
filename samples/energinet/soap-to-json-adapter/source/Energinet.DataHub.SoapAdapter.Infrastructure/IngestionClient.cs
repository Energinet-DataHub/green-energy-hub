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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application.Infrastructure;

namespace Energinet.DataHub.SoapAdapter.Infrastructure
{
    /// <summary>
    /// Takes a JSON stream and sends this to the configured ingestion endpoint
    /// </summary>
    public sealed class IngestionClient : IIngestionClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IngestionClientSettings _settings;

        public IngestionClient(
            IHttpClientFactory clientFactory,
            IngestionClientSettings settings)
        {
            _clientFactory = clientFactory;
            _settings = settings;
        }

        /// <summary>
        /// Takes a JSON stream and sends this to the configured ingestion endpoint
        /// </summary>
        /// <param name="stream">A JSON request stream</param>
        /// <param name="marketDocumentType">MarketDocument type</param>
        /// <returns>A JSON response stream</returns>
        public async Task<Stream> SendAsync(Stream stream, string marketDocumentType)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrWhiteSpace(marketDocumentType))
            {
                throw new ArgumentNullException(nameof(marketDocumentType));
            }

            stream.Position = 0;

            using (var httpRequestMessage = new HttpRequestMessage
            {
                Content = new StreamContent(stream),
                RequestUri = new Uri($"{_settings.Endpoint}/{marketDocumentType}", UriKind.Absolute),
                Method = HttpMethod.Post,
            })
            {
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var httpClient = _clientFactory.CreateClient())
                {
                    var response = await httpClient.SendAsync(httpRequestMessage, CancellationToken.None)
                        .ConfigureAwait(false);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
