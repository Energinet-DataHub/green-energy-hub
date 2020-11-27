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
using NodaTime;

namespace GreenEnergyHub.ValidationReports.Domain.Models
{
    /// <summary>
    /// The main Model used for persisting a single transactions ValidationResults.
    /// </summary>
    public class ValidationResultContainer
    {
        public ValidationResultContainer() { }

        public ValidationResultContainer(Guid correlationId, Instant created, string systemUser, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("value cannot be empty", nameof(id));
            }

            if (string.IsNullOrEmpty(systemUser))
            {
                throw new ArgumentException("value cannot be empty", nameof(systemUser));
            }

            if (created == default)
            {
                throw new ArgumentOutOfRangeException(nameof(created));
            }

            if (correlationId == default)
            {
                throw new ArgumentOutOfRangeException(nameof(correlationId));
            }

            MarketEvaluationPointMrId = id;
            SystemUser = systemUser;
            Created = created;
            CorrelationId = correlationId;
        }

        /// <summary>
        /// The internal CorrelationId used throughout the system
        /// </summary>
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// Creation time and date
        /// </summary>
        public Instant? Created { get; set; }

        /// <summary>
        /// Owner of the data
        /// </summary>
        public string? SystemUser { get; set; }

        /// <summary>
        /// The mRID reference used by outside actors, to keep track of their messages
        /// </summary>
        public string? MarketEvaluationPointMrId { get; set; }

        /// <summary>
        /// List of validation results
        /// </summary>
        #pragma warning disable CA2227 // Modifying existing properties is not supported in .Net Core 3.1 https://github.com/dotnet/runtime/issues/30258
        public List<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();
        #pragma warning restore CA2227

        public void AddResult(ValidationResult result) => ValidationResults.Add(result);

        public void AddResults(IEnumerable<ValidationResult> results) => ValidationResults.AddRange(results);
    }
}
