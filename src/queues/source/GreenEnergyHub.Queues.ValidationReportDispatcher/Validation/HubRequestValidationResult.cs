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
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageTypes.Common;
using NodaTime;

namespace GreenEnergyHub.Queues.ValidationReportDispatcher.Validation
{
    /// <summary>
    /// POCO for a validation results.
    /// </summary>
    public class HubRequestValidationResult : IHubMessage
    {
        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();

        public HubRequestValidationResult(string transactionId)
        {
            Transaction = !string.IsNullOrWhiteSpace(transactionId) ? new Transaction(transactionId) : throw new ArgumentNullException(nameof(transactionId));
            RequestDate = SystemClock.Instance.GetCurrentInstant();
        }

        public Transaction Transaction { get; set; }

        public Instant RequestDate { get; set; }

        public IReadOnlyList<ValidationError> Errors => _validationErrors.AsReadOnly();

        public void Add(ValidationError validationError)
        {
            _validationErrors.Add(validationError);
        }
    }
}
