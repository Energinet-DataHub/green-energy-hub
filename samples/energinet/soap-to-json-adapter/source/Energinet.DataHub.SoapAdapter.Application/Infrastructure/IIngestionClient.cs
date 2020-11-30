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
using System.IO;
using System.Threading.Tasks;

namespace Energinet.DataHub.SoapAdapter.Application.Infrastructure
{
    /// <summary>
    /// Send requests to ingestion
    /// </summary>
    public interface IIngestionClient
    {
        /// <summary>
        /// Send a request to ingestion
        /// </summary>
        /// <param name="stream">Content to be sent.</param>
        /// <param name="marketDocumentType">Name of the document type to be sent.</param>
        /// <returns>A <see cref="Stream"/> with the response from the ingestion service.</returns>
        Task<Stream> SendAsync(Stream stream, string marketDocumentType);
    }
}
