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
using System.Threading;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Synchronous.Application.Requests;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.RequestQueue;

namespace Energinet.DataHub.Ingestion.Synchronous.Application.Handlers
{
    /// <summary>
    /// Class which defines how to handle ChangeOfSupplierRequests.
    /// </summary>
    public class ChangeOfSupplierHandler : HubRequestHandler<ChangeOfSupplierRequest>
    {
        private readonly IRuleEngine<ChangeOfSupplierRequest> _rulesEngine;
        private readonly IHubRequestQueueDispatcher _requestDispatcher;

        /// <summary>
        /// Builds a ChangeOfSupplierHandler which validates messages using a
        /// provided IRuleEngine.
        /// </summary>
        /// <param name="rulesEngine">The IRuleEngine to validate messages with.
        /// </param>
        /// <param name="requestQueueDispatcher">Queue dispatcher to use when request is successfully validated.</param>
        public ChangeOfSupplierHandler(
            IRuleEngine<ChangeOfSupplierRequest> rulesEngine,
            IHubRequestQueueDispatcher requestQueueDispatcher)
        {
            _rulesEngine = rulesEngine;
            _requestDispatcher = requestQueueDispatcher;
        }

        /// <summary>
        /// Validates a given ChangeOfSupplierRequest.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierRequest.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if it is valid.</returns>
        protected override Task<bool> ValidateAsync(ChangeOfSupplierRequest actionData, CancellationToken cancellationToken)
        {
            // TODO: Enable validation when we are settled on a validation engine/methodology
            // return await _rulesEngine.ValidateAsync(actionData).ConfigureAwait(false);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Accepts a ChangeOfSupplierRequest.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierRequest.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the request was successfully accepted.</returns>
        protected override async Task<bool> AcceptAsync(ChangeOfSupplierRequest actionData, CancellationToken cancellationToken)
        {
            await _requestDispatcher.DispatchAsync(actionData).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Creates a response for the ChangeOfSupplierRequest.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierRequest.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A response.</returns>
        protected override Task<IHubResponse> RespondAsync(ChangeOfSupplierRequest actionData, CancellationToken cancellationToken)
        {
            return Task.FromResult<IHubResponse>(new HubResponse());
        }
    }
}
