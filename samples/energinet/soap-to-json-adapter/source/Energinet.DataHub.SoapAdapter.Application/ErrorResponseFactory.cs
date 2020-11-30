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
using System.Xml;
using Energinet.DataHub.SoapAdapter.Application.Dtos;

namespace Energinet.DataHub.SoapAdapter.Application
{
    public class ErrorResponseFactory : IErrorResponseFactory
    {
        private const string SoapPrefix = "SOAP-ENV";
        private const string EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string BodyNamespace = "http://www.w3.org/2001/XMLSchema";

        private static readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings { Async = true, };

        public async Task<Response> CreateAsync(string message, string code = "Client")
        {
            var output = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(output, _xmlWriterSettings))
            {
                await xmlWriter.WriteStartDocumentAsync().ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(SoapPrefix, "Envelope", EnvelopeNamespace).ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(SoapPrefix, "Body", EnvelopeNamespace).ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(SoapPrefix, "Fault", BodyNamespace).ConfigureAwait(false);
                await xmlWriter.WriteElementStringAsync(string.Empty, "faultcode", string.Empty, code).ConfigureAwait(false);
                await xmlWriter.WriteElementStringAsync(string.Empty, "faultstring", string.Empty, message).ConfigureAwait(false);
                await xmlWriter.WriteElementStringAsync(string.Empty, "faultactor", string.Empty, string.Empty).ConfigureAwait(false);
                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);

                await xmlWriter.FlushAsync().ConfigureAwait(false);
            }

            output.Position = 0;

            return new Response(false, output);
        }
    }
}
