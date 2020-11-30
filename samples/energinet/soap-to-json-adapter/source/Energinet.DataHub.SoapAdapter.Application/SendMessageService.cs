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
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application.Converters;
using Energinet.DataHub.SoapAdapter.Application.Dtos;
using Energinet.DataHub.SoapAdapter.Application.Exceptions;
using Energinet.DataHub.SoapAdapter.Application.Infrastructure;
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.Domain.Validation;

namespace Energinet.DataHub.SoapAdapter.Application
{
    /// <inheritdoc cref="ISendMessageService"/>
    public class SendMessageService : ISendMessageService
    {
        private readonly RsmValidationParser _validationParser;
        private readonly IRequestConverterFactory _requestConverterFactory;
        private readonly IResponseConverterFactory _responseConverterFactory;
        private readonly IErrorResponseFactory _errorResponseFactory;
        private readonly IIngestionClient _client;

        public SendMessageService(
            RsmValidationParser validationParser,
            IRequestConverterFactory requestConverterFactory,
            IResponseConverterFactory responseConverterFactory,
            IErrorResponseFactory errorResponseFactory,
            IIngestionClient client)
        {
            _validationParser = validationParser;
            _requestConverterFactory = requestConverterFactory;
            _responseConverterFactory = responseConverterFactory;
            _errorResponseFactory = errorResponseFactory;
            _client = client;
        }

        public async Task<Response> HandleSendMessageAsync(Request request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var context = await _validationParser.ParseAsync(request.Content).ConfigureAwait(false);

            try
            {
                if (!Validate(context.RsmDocumentType.IsValid()))
                {
                    // TODO: refactor validation when we agree upon a framework for that.
                    throw new SoapAdapterException(context.MessageReference, "Validation error");
                }

                await using (var streamForIngestion = await ConvertToInternalAsync(request.Content, context).ConfigureAwait(false))
                {
                    var internalResponse = await _client.SendAsync(streamForIngestion, context.RsmDocumentType.Value).ConfigureAwait(false);
                    var response = await ConvertFromInternalAsync(internalResponse, context).ConfigureAwait(false);
                    return new Response(true, response);
                }
            }
            catch (UnknownConverterException)
            {
                return await _errorResponseFactory.CreateAsync($"B2B001:{context.MessageReference}").ConfigureAwait(false);
            }
        }

        private static bool Validate(bool isValid)
        {
            return isValid;
        }

        private async Task<Stream> ConvertToInternalAsync(Stream stream, Context context)
        {
            var converter = _requestConverterFactory.GetConverter(context.RsmDocumentType.Value);

            var output = new MemoryStream();
            await converter.ConvertAsync(stream, output).ConfigureAwait(false);
            return output;
        }

        private async Task<Stream> ConvertFromInternalAsync(Stream stream, Context context)
        {
            var converter = _responseConverterFactory.GetConverter(context.RsmDocumentType.Value);

            var output = new MemoryStream();
            await converter.ConvertAsync(stream, output, context.MessageReference).ConfigureAwait(false);
            return output;
        }
    }
}
