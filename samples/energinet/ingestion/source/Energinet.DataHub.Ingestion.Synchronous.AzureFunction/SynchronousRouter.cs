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
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Application;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.MessageRouting;
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
    public class SynchronousRouter
    {
        private const string FunctionName = nameof(SynchronousRouter);

        private readonly IHubMessageTypeMap _resolver;
        private readonly IHubRehydrator _rehydrator;
        private readonly IHubMessageBulkMediator _bulkMediator;

        /// <summary>
        /// Creates an instance of a RequestRouter using a given resolver.
        /// </summary>
        /// <param name="resolver">The IHubMessageTypeMap to use to figure out
        /// where to send what messages.</param>
        /// <param name="rehydrator">Rehydrate a message to a message type</param>
        /// <param name="bulkMediator">Service for dispatching collection of
        /// messages.</param>
        public SynchronousRouter(
            IHubMessageTypeMap resolver,
            IHubRehydrator rehydrator,
            IHubMessageBulkMediator bulkMediator)
        {
            _resolver = resolver;
            _rehydrator = rehydrator;
            _bulkMediator = bulkMediator;
        }

        /// <summary>
        /// The Azure Functions endpoint.
        /// </summary>
        /// <returns>The HTTP result of running this function.</returns>
        [FunctionName(FunctionName)]
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

            logger.LogInformation($"{FunctionName} started processing a request.");

            var requestType = _resolver.GetTypeByCategory(category);
            if (requestType == null)
            {
                return new NotFoundResult();
            }

            var hubRequests = await _rehydrator.RehydrateCollectionAsync(httpRequest.Body, requestType).ConfigureAwait(false);
            if (hubRequests == null)
            {
                return new BadRequestObjectResult("Invalid request message.");
            }

            // TODO: Downcasting to CustomHubResponse should not occur.
            // In fact, CustomHubResponse should not exist at all; either IHubResponse or HubResponse should define/implement ValidationResults
            var response = await _bulkMediator.DispatchAsync(hubRequests).ConfigureAwait(false) as CustomHubResponse;
            return new OkObjectResult(response?.ValidationResults)
            {
                StatusCode = StatusCodes.Status202Accepted,
            };
        }
    }
}
