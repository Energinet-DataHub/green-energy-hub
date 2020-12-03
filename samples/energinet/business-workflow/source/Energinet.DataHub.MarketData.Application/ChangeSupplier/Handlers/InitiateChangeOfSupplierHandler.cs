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
using GreenEnergyHub.Messaging.Dispatching;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.MarketData.Application.ChangeSupplier.Handlers
{
    public class InitiateChangeOfSupplierHandler : HubCommandHandler<InitiateChangeOfSupplier>
    {
        private readonly ILogger<InitiateChangeOfSupplierHandler> _logger;

        public InitiateChangeOfSupplierHandler(
            ILogger<InitiateChangeOfSupplierHandler> logger)
        {
            _logger = logger;
        }

        protected override Task AcceptAsync(InitiateChangeOfSupplier actionData, CancellationToken cancellationToken)
        {
            // TODO: Map to domain model
            var domainModel = actionData;

            // TODO: Save in DB
            _logger.LogInformation("Should save to DB here.");
            return Task.FromResult(false);
        }
    }
}
