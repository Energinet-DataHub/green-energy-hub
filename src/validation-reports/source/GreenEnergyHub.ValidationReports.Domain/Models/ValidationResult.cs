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

namespace GreenEnergyHub.ValidationReports.Domain.Models
{
    public class ValidationResult
    {
        public ValidationResult()
        {
        }

        public ValidationResult(
            string validationId,
            bool passedValidation,
            IEnumerable<KeyValuePair<string, string>>? additionalData = null)
        {
            if (string.IsNullOrEmpty(validationId))
            {
                throw new ArgumentException("value cannot be empty", nameof(validationId));
            }

            ValidationId = validationId;
            PassedValidation = passedValidation;
            AdditionalData = additionalData != null ? new Dictionary<string, string>(additionalData) : new Dictionary<string, string>();
        }

        /// <summary>
        /// A single validation result
        /// </summary>
        public bool PassedValidation { get; set; }

        /// <summary>
        /// The validation id.
        /// </summary>
        public string? ValidationId { get; set; }

        /// <summary>
        /// Additional data for a validation result
        /// </summary>
        public Dictionary<string, string>? AdditionalData { get; }

        public static ValidationResult Success(
            string validationId,
            params KeyValuePair<string, string>[] additionalData)
            => new ValidationResult(validationId, true, additionalData);

        public static ValidationResult Failure(
            string validationId,
            params KeyValuePair<string, string>[] additionalData)
            => new ValidationResult(validationId, false, additionalData);
    }
}
