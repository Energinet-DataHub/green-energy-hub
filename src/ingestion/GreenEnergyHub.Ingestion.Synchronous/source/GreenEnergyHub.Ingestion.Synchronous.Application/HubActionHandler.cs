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

namespace GreenEnergyHub.Ingestion.Synchronous.Application
{
    /// <summary>
    /// A Handler for a given IHubActionRequest.
    /// </summary>
    /// <typeparam name="TRequest">The type of IHubActionRequest to
    /// handle.</typeparam>
    public abstract class HubActionHandler<TRequest> : IHubActionHandler<TRequest>
      where TRequest : IHubActionRequest
    {
        /// <summary>
        /// Validate any properties of the message before accepting.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>True if it is a valid message.</returns>
        public abstract Task<bool> ValidateAsync(TRequest actionData);

        /// <summary>
        /// Accept the message and perform any stateful data transformation,
        /// system updating, etc.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>True if message successfully accepted.</returns>
        public abstract Task<bool> AcceptAsync(TRequest actionData);

        /// <summary>
        /// Send information about acceptance and validity to some outside
        /// observer.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>A HubActionResponse.</returns>
        public abstract Task<IHubActionResponse> RespondAsync(TRequest actionData);

        /// <summary>
        /// The default way to handle requests: Validate, Accept, and Respond.
        ///
        /// Implementing classes can extend HubActionHandler to leverage this
        /// existing algorithm, or IHubActionHandler to implement their own
        /// functionality.
        ///
        /// We recommend an abstract class like this one to allow for similar
        /// behaviors to reuse code.
        /// </summary>
        /// <param name="request">The message to handle.</param>
        /// <param name="cancellationToken">A cancellationToken.</param>
        /// <returns>A HubActionResponse.</returns>
        public virtual async Task<IHubActionResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            if (!await ValidateAsync(request).ConfigureAwait(false))
            {
                return new HubActionResponse(false, new string[] { "validation failed" });
            }

            if (!await AcceptAsync(request).ConfigureAwait(false))
            {
                // TODO better error handling behavior
                throw new Exception("Error from outside service:");
            }

            return await RespondAsync(request).ConfigureAwait(false);
        }
    }
}
