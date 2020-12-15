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

namespace Energinet.DataHub.Ingestion.Domain.Messages
{
    /// <summary>
    /// Contains the time series points
    /// </summary>
    public class Period
    {
        /// <summary>
        /// The resolution of the intervals eg. PT15M, PT1H
        /// </summary>
        public string? Resolution { get; set; }

        /// <summary>
        /// The time interval of the series, with start and end times.
        /// </summary>
        public TimeInterval? TimeInterval { get; set; }

        /// <summary>
        /// All the time intervals
        /// </summary>
        public List<Point>? Points { get; } = new List<Point>();
    }
}
