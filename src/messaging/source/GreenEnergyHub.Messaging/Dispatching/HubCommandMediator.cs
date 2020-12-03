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
    public class HubCommandMediator : IHubCommandMediator
    {
        private readonly IMediator _mediator;

        public HubCommandMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync<TCommand>(TCommand request)
            where TCommand : IHubMessage
        {
            var targetType = typeof(HubCommand<>);

            var instance = Activator.CreateInstance(targetType.MakeGenericType(request.GetType()), request);

            if (instance == null)
            {
                throw new Exception(); // TODO: better exception needed
            }

            await _mediator.Send(instance).ConfigureAwait(false);
        }
    }
}
