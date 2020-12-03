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

namespace GreenEnergyHub.Messaging.Dispatching
{
    /// <summary>
    /// A Handler for <see cref="BaseRequestHandler{TRequest,TResponse}"/>.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="IHubMessage"/> to handle.</typeparam>
    public class HubRequestHandler<TRequest> : BaseRequestHandler<TRequest, IHubResponse>
        where TRequest : IHubMessage
    {
        protected HubRequestHandler() { }

        /// <summary>
        /// Validate any properties of the message before accepting.
        /// </summary>
        /// <param name="request">The message.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if it is a valid message.</returns>
        protected virtual Task<bool> ValidateAsync(TRequest request, CancellationToken cancellationToken) => Task.FromResult(true);

        /// <summary>
        /// Accept the message and perform any stateful data transformation,
        /// system updating, etc.
        /// </summary>
        /// <param name="request">The message.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>True if message successfully accepted.</returns>
        protected virtual Task<bool> AcceptAsync(TRequest request, CancellationToken cancellationToken) => Task.FromResult(true);

        /// <summary>
        /// Send information about acceptance and validity to some outside
        /// observer.
        /// </summary>
        /// <param name="request">The message.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A HubActionResponse.</returns>
        protected virtual Task<IHubResponse> RespondAsync(TRequest request, CancellationToken cancellationToken) => Task.FromResult<IHubResponse>(new HubResponse());

        /// <summary>
        /// Called when the handle method experiences an unexpected exception.
        /// </summary>
        /// <param name="innerException">The exception that was thrown during Handle().</param>
        protected virtual Task<IHubResponse> OnErrorAsync(Exception innerException)
        {
            // Throw by default, but permit customer to override error handling
            // action
            throw innerException;
        }

        /// <summary>
        /// The default way to handle requests: Validate, Accept, and Respond.
        ///
        /// We recommend an abstract class like this one to allow for similar
        /// behaviors to reuse code.
        /// </summary>
        /// <param name="request">The message to handle.</param>
        /// <param name="cancellationToken">A cancellationToken.</param>
        /// <returns>An IHubActionResponse.</returns>
        private protected override async Task<IHubResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!await ValidateAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    return new HubResponse(false, new string[] { "validation failed" });
                }

                if (!await AcceptAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    return new HubResponse(false, new string[] { "accept failed" });
                }

                return await RespondAsync(request, cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                return await OnErrorAsync(e).ConfigureAwait(false);
            }
#pragma warning restore CA1031
        }
    }
}
