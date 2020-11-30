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
using System.Net.Http;
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application;
using Energinet.DataHub.SoapAdapter.Application.Dtos;
using Energinet.DataHub.SoapAdapter.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.SoapAdapter.EntryPoint
{
    public class SendMessageFunction
    {
        private readonly ISendMessageService _sendMessageService;
        private readonly IErrorResponseFactory _errorResponseFactory;

        public SendMessageFunction(
            ISendMessageService sendMessageService,
            IErrorResponseFactory errorResponseFactory)
        {
            _sendMessageService = sendMessageService;
            _errorResponseFactory = errorResponseFactory;
        }

        [FunctionName("SendMessage")]
        public async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest httpRequest,
            ILogger logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (httpRequest is null)
            {
                logger.LogError($"{nameof(httpRequest)} is null in SendMessage", httpRequest);
                throw new ArgumentNullException(nameof(httpRequest));
            }

            try
            {
                // Use a real correlation id instead of Guid.NewGuid().ToString()
                var request = new Request(Guid.NewGuid().ToString(), httpRequest.Body);
                var response = await _sendMessageService
                    .HandleSendMessageAsync(request)
                    .ConfigureAwait(false);
                return response.AsHttpResponseMessage();
            }
            catch (SoapAdapterException exception)
            {
                // TODO: correlation id?
                logger.LogError(exception, "Error in SendMessage");
                var response = await _errorResponseFactory
                    .CreateAsync($"{exception.ErrorMessage}:{exception.MessageReference}")
                    .ConfigureAwait(false);
                return response.AsHttpResponseMessage();
            }
#pragma warning disable CA1031 // This is the point where we want to catch unknown exceptions
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Error in SendMessage");
                var response = await _errorResponseFactory
                    .CreateAsync("B2B-900", "Server")
                    .ConfigureAwait(false);
                return response.AsHttpResponseMessage();
#pragma warning disable CA1031
            }
        }
    }
}
