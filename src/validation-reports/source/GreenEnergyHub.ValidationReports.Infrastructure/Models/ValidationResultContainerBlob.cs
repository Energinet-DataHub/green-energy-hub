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
using System.Text.Json;
using GreenEnergyHub.ValidationReports.Domain.Models;
using Microsoft.Azure.Cosmos.Table;
using NodaTime;

namespace GreenEnergyHub.ValidationReports.Infrastructure.Models
{
    public class ValidationResultContainerBlob : TableEntity
    {
        public ValidationResultContainerBlob(
            string? partitionKey,
            string? rowKey,
            Guid? correlationId,
            Instant? created,
            string? systemUser,
            string? id,
            string? validationResults)
            : base(partitionKey, rowKey)
        {
            CorrelationId = correlationId;
            Created = created;
            SystemUser = systemUser;
            MarketEvaluationPointMRid = id;
            ValidationResults = validationResults;
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
        public string? MarketEvaluationPointMRid { get; set; }

        /// <summary>
        /// A JSON string containing the results
        /// </summary>
        public string? ValidationResults { get; set; }

        /// <summary>
        /// Convert a report to a table entity that can be stored in table storage
        /// </summary>
        /// <param name="resultContainer">Validation report that we want to have converted</param>
        /// <returns>A custom table entity object</returns>
        public static ValidationResultContainerBlob FromReport(ValidationResultContainer resultContainer)
        {
            if (resultContainer == null)
            {
                throw new ArgumentNullException(nameof(resultContainer));
            }

            return new ValidationResultContainerBlob(
                resultContainer.SystemUser,
                resultContainer.MarketEvaluationPointMrId,
                resultContainer.CorrelationId,
                resultContainer.Created,
                resultContainer.SystemUser,
                resultContainer.MarketEvaluationPointMrId,
                JsonSerializer.Serialize(resultContainer.ValidationResults));
        }
    }
}
