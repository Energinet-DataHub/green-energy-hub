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
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.RequestRouting;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.MarketData.EntryPoint
{
    public class QueueSubscriber
    {
        private readonly IHubRequestTypeMap _resolver;
        private readonly IHubRehydrate _rehydrate;
        private readonly IHubRequestDispatcher _dispatcher;

        public QueueSubscriber(
            IHubRequestTypeMap resolver,
            IHubRehydrate rehydrate,
            IHubRequestDispatcher dispatcher)
        {
            _resolver = resolver;
            _rehydrate = rehydrate;
            _dispatcher = dispatcher;
        }

        [FunctionName("QueueSubscriber")]
        public async Task RunAsync(
            [ServiceBusTrigger("%queueName%")] QueueMessage message,
            ILogger logger)
        {
            if (message == null || message.RequestType == null || message.Request == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");
            logger.LogInformation($"With request type: {message.RequestType}");
            logger.LogInformation($"With request value: {message.Request}");

            var requestType = _resolver.GetTypeByCategory(message.RequestType);
            if (requestType == null)
            {
                throw new ArgumentNullException(message.RequestType);
            }

            var hubRequest = await _rehydrate.RehydrateAsync(message.Request, requestType).ConfigureAwait(false);
            if (hubRequest == null)
            {
                throw new ArgumentException(nameof(message.Request));
            }

            await _dispatcher.DispatchAsync(hubRequest).ConfigureAwait(false);

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
