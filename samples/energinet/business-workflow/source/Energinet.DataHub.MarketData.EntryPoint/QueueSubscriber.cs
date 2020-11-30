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
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.MarketData.EntryPoint
{
    public static class QueueSubscriber
    {
        [FunctionName("QueueSubscriber")]
        public static async Task RunAsync(
            [ServiceBusTrigger("%queueName%")] QueueMessage message,
            ILogger logger)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {message}");
            logger.LogInformation($"With request type: {message.RequestType}");
            logger.LogInformation($"With request value: {message.Request}");
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
