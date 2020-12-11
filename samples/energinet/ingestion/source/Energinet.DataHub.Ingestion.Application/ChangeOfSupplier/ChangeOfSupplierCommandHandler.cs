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
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.MessageQueue;

namespace Energinet.DataHub.Ingestion.Application.ChangeOfSupplier
{
    /// <summary>
    /// Class which defines how to handle ChangeOfSupplierMessages.
    /// </summary>
    public class ChangeOfSupplierCommandHandler : HubCommandHandler<ChangeOfSupplierMessage>
    {
        private readonly IRuleEngine<ChangeOfSupplierMessage> _rulesEngine;
        private readonly IHubMessageServiceBusDispatcher _messageDispatcher;

        /// <summary>
        /// Builds a ChangeOfSupplierCommandHandler which validates messages using a
        /// provided IRuleEngine and dispatches valid messages to a Service Bus.
        /// </summary>
        /// <param name="rulesEngine">The IRuleEngine to validate messages with.
        /// </param>
        /// <param name="messageServiceBusDispatcher">Service Bus dispatcher to use when request is successfully validated.</param>
        public ChangeOfSupplierCommandHandler(
            IRuleEngine<ChangeOfSupplierMessage> rulesEngine,
            IHubMessageServiceBusDispatcher messageServiceBusDispatcher)
        {
            _rulesEngine = rulesEngine;
            _messageDispatcher = messageServiceBusDispatcher;
        }

        /// <summary>
        /// Validates a given ChangeOfSupplierMessage.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if it is valid.</returns>
        protected override Task<bool> ValidateAsync(ChangeOfSupplierMessage actionData, CancellationToken cancellationToken)
        {
            // TODO: Enable validation when we are settled on a validation engine/methodology
            // return await _rulesEngine.ValidateAsync(actionData).ConfigureAwait(false);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Accepts a ChangeOfSupplierMessage.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierMessage.</param>
        /// <param name="cancellationToken"></param>
        protected override async Task AcceptAsync(ChangeOfSupplierMessage actionData, CancellationToken cancellationToken)
        {
            await _messageDispatcher.DispatchAsync(actionData).ConfigureAwait(false);
        }

        /// <summary>
        /// Rejects a given ChangeOfSupplierMessage.
        /// </summary>
        /// <param name="actionData">The ChangeOfSupplierMessage.</param>
        /// <param name="cancellationToken"></param>
        protected override Task RejectAsync(ChangeOfSupplierMessage actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the handle method experiences an unexpected exception.
        /// </summary>
        /// <param name="innerException">The exception that was thrown during Handle().</param>
        protected override Task OnErrorAsync(Exception innerException)
        {
            // TODO: On error, send message to some dead-letter queue
            throw innerException;
        }
    }
}
