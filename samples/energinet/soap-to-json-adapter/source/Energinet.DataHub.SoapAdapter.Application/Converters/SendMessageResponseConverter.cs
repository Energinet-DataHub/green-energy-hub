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
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    /// <summary>
    /// Convert a JSON response to a SOAP XML response
    /// </summary>
    public sealed class SendMessageResponseConverter : IResponseConverter
    {
        private const string SoapPrefix = "SOAP-ENV";
        private const string EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
        private const string BodyNamespace = "http://www.w3.org/2001/XMLSchema";

        private readonly XmlWriterSettings _xmlWriterSettings;

        public SendMessageResponseConverter()
        {
            _xmlWriterSettings = new XmlWriterSettings
            {
                Async = true,
            };
        }

        public async ValueTask ConvertAsync(Stream input, Stream output, string identifier)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            using (var xmlWriter = XmlWriter.Create(output, _xmlWriterSettings))
            {
                await xmlWriter.WriteStartDocumentAsync().ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(SoapPrefix, "Envelope", EnvelopeNamespace).ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(SoapPrefix, "Body", EnvelopeNamespace).ConfigureAwait(false);
                await xmlWriter.WriteStartElementAsync(string.Empty, "SendMessageResponse", BodyNamespace).ConfigureAwait(false);
                await xmlWriter.WriteElementStringAsync(string.Empty, "MessageId", BodyNamespace, identifier).ConfigureAwait(false);

                using (var jsonDocument = await JsonDocument.ParseAsync(input).ConfigureAwait(false))
                {
                    foreach (var validationResult in jsonDocument.RootElement.EnumerateArray())
                    {
                        await xmlWriter.WriteStartElementAsync(string.Empty, "Transaction", BodyNamespace).ConfigureAwait(false);
                        var transaction = validationResult.GetProperty("transaction");
                        await xmlWriter.WriteElementStringAsync(string.Empty, "Mrid", BodyNamespace, transaction.GetProperty("mrid").GetString()).ConfigureAwait(false);

                        foreach (var error in validationResult.GetProperty("errors").EnumerateArray())
                        {
                            await xmlWriter.WriteStartElementAsync(string.Empty, "Error", BodyNamespace).ConfigureAwait(false);
                            await xmlWriter.WriteElementStringAsync(string.Empty, "Code", BodyNamespace, error.GetProperty("code").GetString()).ConfigureAwait(false);
                            await xmlWriter.WriteElementStringAsync(string.Empty, "Message", BodyNamespace, error.GetProperty("message").GetString()).ConfigureAwait(false);
                            await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                        }

                        await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                    }
                }

                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);
                await xmlWriter.WriteEndElementAsync().ConfigureAwait(false);

                await xmlWriter.FlushAsync().ConfigureAwait(false);

                output.Position = 0;
            }
        }
    }
}
