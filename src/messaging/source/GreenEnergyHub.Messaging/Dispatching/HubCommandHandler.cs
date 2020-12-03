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
    public class HubCommandHandler<TCommand> : BaseCommandHandler<TCommand>
        where TCommand : IHubMessage
    {
        protected HubCommandHandler() { }

        protected virtual Task<bool> ValidateAsync(TCommand actionData, CancellationToken cancellationToken) =>
            Task.FromResult<bool>(true);

        protected virtual Task AcceptAsync(TCommand actionData, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        protected virtual Task RejectAsync(TCommand actionData, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        /// <summary>
        /// Called when the handle method experiences an unexpected exception.
        /// </summary>
        /// <param name="innerException">The exception that was thrown during Handle().</param>
        protected virtual Task OnErrorAsync(Exception innerException)
        {
            // Throw by default, but permit customer to override error handling
            // action
            throw innerException;
        }

        private protected override async Task HandleAsync(TCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await ValidateAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await AcceptAsync(request, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await RejectAsync(request, cancellationToken).ConfigureAwait(false);
                }
            }
#pragma warning disable CA1031
            catch (Exception e)
            {
                await OnErrorAsync(e).ConfigureAwait(false);
            }
#pragma warning restore CA1031
        }
    }
}
