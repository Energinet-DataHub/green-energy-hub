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

using System.Threading.Tasks;
using MediatR;

namespace GreenEnergyHub.Ingestion
{
    /// <summary>
    /// A Handler for IHubActions.
    /// Ties system to <a href="https://github.com/jbogard/MediatR">MediatR</a>.
    /// </summary>
    /// <typeparam name="TRequest">The IHubActionRequest to handle.</typeparam>
    public interface IHubActionHandler<in TRequest> : IRequestHandler<TRequest, IHubActionResponse>
      where TRequest : IHubActionRequest
    {
        /// <summary>
        /// Validate any properties of the message before accepting.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>True if it is a valid message.</returns>
        Task<bool> ValidateAsync(TRequest actionData);

        /// <summary>
        /// Accept the message and perform any stateful data transformation,
        /// system updating, etc.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>True if message successfully accepted.</returns>
        Task<bool> AcceptAsync(TRequest actionData);

        /// <summary>
        /// Send information about acceptance and validity to some outside
        /// observer.
        /// </summary>
        /// <param name="actionData">The message.</param>
        /// <returns>A HubActionResponse.</returns>
        Task<IHubActionResponse> RespondAsync(TRequest actionData);
    }
}
