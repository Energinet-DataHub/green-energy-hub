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
using System.Collections.Generic;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Synchronous.Application;
using Energinet.DataHub.Ingestion.Synchronous.Infrastructure;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.RequestRouting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Synchronous.AzureFunction
{
    /// <summary>
    /// Class which gives the only Azure Function endpoint which handles
    /// requests by getting endpoints from a provided resolver.
    /// </summary>
    public class RequestRouter
    {
        private readonly IHubRequestTypeMap _resolver;
        private readonly IHubRehydrate _rehydrate;
        private readonly IHubRequestBulkDispatcher _bulkDispatcher;

        /// <summary>
        /// Creates an instance of a RequestRouter using a given resolver.
        /// </summary>
        /// <param name="resolver">The IEndpointResolver to use to figure out
        /// where to send what requests.</param>
        /// <param name="rehydrate">Rehydrate a message to a request type</param>
        /// <param name="bulkDispatcher">Service for dispatching collection of requests.</param>
        public RequestRouter(
            IHubRequestTypeMap resolver,
            IHubRehydrate rehydrate,
            IHubRequestBulkDispatcher bulkDispatcher)
        {
            _resolver = resolver;
            _rehydrate = rehydrate;
            _bulkDispatcher = bulkDispatcher;
        }

        /// <summary>
        /// The Azure Functions endpoint.
        /// </summary>
        /// <returns>The HTTP result of running this function.</returns>
        [FunctionName("Router")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{category:alpha}")] HttpRequest httpRequest,
            ILogger logger,
            string category)
        {
            if (httpRequest is null)
            {
                throw new ArgumentNullException(nameof(httpRequest));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestType = _resolver.GetTypeByCategory(category);
            if (requestType == null)
            {
                return new NotFoundResult();
            }

            var hubRequests = await _rehydrate.RehydrateCollectionAsync(httpRequest.Body, requestType).ConfigureAwait(false);
            if (hubRequests == null)
            {
                return new BadRequestObjectResult("Invalid request message.");
            }

            // TODO: Downcasting to CustomHubResponse should not occur.
            // In fact, CustomHubResponse should not exist at all; either IHubResponse or HubResponse should define/implement ValidationResults
            var response = await _bulkDispatcher.DispatchAsync(hubRequests).ConfigureAwait(false) as CustomHubResponse;
            return new OkObjectResult(response?.ValidationResults)
            {
                StatusCode = StatusCodes.Status202Accepted,
            };
        }
    }
}
