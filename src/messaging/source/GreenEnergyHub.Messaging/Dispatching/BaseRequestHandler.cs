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
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace GreenEnergyHub.Messaging.Dispatching
{
    /// <summary>
    /// Ties system to <a href="https://github.com/jbogard/MediatR">MediatR</a>.
    /// </summary>
    /// <typeparam name="TRequest">The request to handle</typeparam>
    /// <typeparam name="TResponse">The response</typeparam>
    public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<HubRequest<TRequest, TResponse>, TResponse>
        where TRequest : IHubMessage
        where TResponse : IHubResponse
    {
#pragma warning disable CA1033
        async Task<TResponse> IRequestHandler<HubRequest<TRequest, TResponse>, TResponse>.Handle(HubRequest<TRequest, TResponse> hubRequest, CancellationToken cancellationToken)
        {
            return await HandleAsync(hubRequest.Request, cancellationToken).ConfigureAwait(false);
        }
#pragma warning restore CA1033

        private protected abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
