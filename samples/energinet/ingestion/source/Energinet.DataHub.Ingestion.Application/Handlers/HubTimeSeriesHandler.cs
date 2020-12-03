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
using Energinet.DataHub.Ingestion.Application.Messages;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.MessageQueue;

namespace Energinet.DataHub.Ingestion.Application.Handlers
{
    /// <summary>
    /// Class which defines how to handle HubTimeSeriesMessages.
    /// </summary>
    public class HubTimeSeriesHandler : HubCommandHandler<HubTimeSeriesMessage>
    {
        private readonly IRuleEngine<HubTimeSeriesMessage> _rulesEngine;
        private readonly IHubMessageQueueDispatcher _messageDispatcher;

        /// <summary>
        /// Builds a HubTimeSeriesHandler which validates messages using a
        /// provided IRuleEngine.
        /// </summary>
        /// <param name="rulesEngine">The IRuleEngine to validate messages with.
        /// </param>
        /// <param name="messageQueueDispatcher">Queue dispatcher to use when request is successfully validated.</param>
        public HubTimeSeriesHandler(
            IRuleEngine<HubTimeSeriesMessage> rulesEngine,
            IHubMessageQueueDispatcher messageQueueDispatcher)
        {
            _rulesEngine = rulesEngine;
            _messageDispatcher = messageQueueDispatcher;
        }

        /// <summary>
        /// Validates a given HubTimeSeriesMessage.
        /// </summary>
        /// <param name="actionData">The HubTimeSeriesMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if it is valid.</returns>
        protected override Task<bool> ValidateAsync(HubTimeSeriesMessage actionData, CancellationToken cancellationToken)
        {
            // TODO: Enable validation when we are settled on a validation engine/methodology
            // return await _rulesEngine.ValidateAsync(actionData).ConfigureAwait(false);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Accepts a HubTimeSeriesMessage.
        /// </summary>
        /// <param name="actionData">The HubTimeSeriesMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the request was successfully accepted.</returns>
        protected override async Task AcceptAsync(HubTimeSeriesMessage actionData, CancellationToken cancellationToken)
        {
            await _messageDispatcher.DispatchAsync(actionData).ConfigureAwait(false);
        }

        /// <summary>
        /// Rejects a given HubTimeSeriesMessage.
        /// </summary>
        /// <param name="actionData">The HubTimeSeriesMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True if it is valid.</returns>
        protected override Task RejectAsync(HubTimeSeriesMessage actionData, CancellationToken cancellationToken)
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
