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

namespace GreenEnergyHub.ValidationReport.Domain.Models
{
    /// <summary>
    /// The main Model used for persisting a single transactions ValidationResults.
    /// </summary>
    public class ValidationResultContainer
    {
        /// <summary>
        /// The internal CorrelationId used throughout the system
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// The mRID reference used by outside actors, to keep track of their messages
        /// </summary>
        public string Mrid { get; set; } = null!;

        /// <summary>
        /// List of validation results
        /// </summary>
        public IEnumerable<Models.ValidationResult> ValidationResults { get; set; } = null!;
    }
}
