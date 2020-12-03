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
using MediatR;

namespace GreenEnergyHub.Messaging.Dispatching
{
    /// <inheritdoc cref="IHubRequestMediator"/>
    public class HubRequestMediator : IHubRequestMediator
    {
        private readonly IMediator _mediator;

        public HubRequestMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IHubResponse> DispatchAsync<TRequest>(TRequest request)
            where TRequest : IHubMessage
        {
            var targetType = typeof(HubRequest<,>);

            var instance = Activator.CreateInstance(targetType.MakeGenericType(request.GetType(), typeof(IHubResponse)), request);

            if (instance == null)
            {
                throw new Exception(); // TODO: More descriptive exception
            }

            var response = await _mediator.Send(instance).ConfigureAwait(false);
            if (response == null)
            {
                throw new Exception(); // TODO: More descriptive exception
            }

            return (IHubResponse)response;
        }
    }
}
