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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.MessageRouting;
using GreenEnergyHub.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Asynchronous.AzureFunction
{
    /// <summary>
    /// Class which gives the async queue-triggered Azure Function.
    /// </summary>
    public class CommandRouter
    {
        private readonly IHubMessageTypeMap _resolver;
        private readonly IHubRehydrator _rehydrator;
        private readonly IHubCommandMediator _commandDispatcher;

        /// <summary>
        /// Creates an instance of a CommandRouter using a given resolver.
        /// </summary>
        /// <param name="resolver">The IHubMessageTypeMap to use to figure out
        /// where to send what messages.</param>
        /// <param name="rehydrator">Rehydrates a message to a message type</param>
        /// <param name="commandDispatcher">Service for dispatching collection
        /// of requests.</param>
        public CommandRouter(
            IHubMessageTypeMap resolver,
            IHubRehydrator rehydrator,
            IHubCommandMediator commandDispatcher)
        {
            _resolver = resolver;
            _rehydrator = rehydrator;
            _commandDispatcher = commandDispatcher;
        }

        /// <summary>
        /// Reads from an input queue and routes messages to different output
        /// queues.
        /// </summary>
        /// <param name="eventData">The message read.</param>
        /// <param name="logger">A logger.</param>
        [FunctionName("Router")]
        public async Task RunAsync(
            [EventHubTrigger("%REQUEST_QUEUE_CONSUMER_GROUP%", Connection = "REQUEST_QUEUE_CONNECTION_STRING")] MessageEnvelope eventData,
            ILogger logger)
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            logger.LogInformation("C# Event Hub trigger function processed a request.");

            var requestType = Type.GetType(eventData.MessageType);
            if (requestType == null)
            {
                return; // TODO: go to error queue or storage?
            }

            using (var body = new MemoryStream(Encoding.UTF8.GetBytes(eventData.Message)))
            {
                var hubRequest = await _rehydrator.RehydrateAsync(body, requestType).ConfigureAwait(false);
                if (hubRequest == null)
                {
                    return; // TODO: go to error queue or storage?
                }

                await _commandDispatcher.DispatchAsync(hubRequest).ConfigureAwait(false);
            }
        }
    }
}
