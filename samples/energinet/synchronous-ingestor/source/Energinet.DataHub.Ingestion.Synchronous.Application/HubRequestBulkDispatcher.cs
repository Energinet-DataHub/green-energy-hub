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
using System.Collections.Generic;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Synchronous.Application.Validation;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.Dispatching;

namespace Energinet.DataHub.Ingestion.Synchronous.Application
{
    // TODO: This service could be placed in GreenEnergyHub.Messaging module.
    public class HubRequestBulkDispatcher : IHubRequestBulkDispatcher
    {
        private readonly IHubRequestDispatcher _hubRequestDispatcher;

        public HubRequestBulkDispatcher(IHubRequestDispatcher hubRequestDispatcher)
        {
            _hubRequestDispatcher = hubRequestDispatcher;
        }

        public async Task<IHubResponse> DispatchAsync(IEnumerable<IHubRequest> hubRequests)
        {
            if (hubRequests == null)
            {
                throw new ArgumentNullException(nameof(hubRequests));
            }

            var validationResults = new List<HubRequestValidationResult>();
            foreach (var hubRequest in hubRequests)
            {
                var result = await _hubRequestDispatcher.DispatchAsync(hubRequest).ConfigureAwait(false);
                var validationResult = new HubRequestValidationResult(hubRequest.Transaction.MRid);

                result.Errors.ForEach(error => validationResult.Add(new ValidationError("UnknownCode", error)));
                validationResults.Add(validationResult);
            }

            return new CustomHubResponse(validationResults);
        }
    }
}
