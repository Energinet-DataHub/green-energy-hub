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

using System.Collections.Generic;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Queues.ValidationReportDispatcher.Validation;

namespace Energinet.DataHub.Ingestion.Application
{
    // TODO: Should be replaced by HubResponse. TBD.
    public class CustomHubResponse : IHubResponse
    {
        private readonly List<HubRequestValidationResult> _validationResults;

        public CustomHubResponse(IEnumerable<HubRequestValidationResult> validationResults)
        {
            _validationResults = new List<HubRequestValidationResult>(validationResults);
        }

        public IReadOnlyList<HubRequestValidationResult> ValidationResults => _validationResults.AsReadOnly();

        public bool IsSuccessful { get; set; } = true;

        public List<string> Errors => new List<string>();
    }
}
