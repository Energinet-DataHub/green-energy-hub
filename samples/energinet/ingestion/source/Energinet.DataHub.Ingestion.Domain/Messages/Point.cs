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

namespace Energinet.DataHub.Ingestion.Domain.Messages
{
    /// <summary>
    /// Point contains a single measurement, also known as a atomic value
    /// </summary>
    public class Point
    {
        /// <summary>
        /// Position is the measurement's relative order in the time series in which is was received.
        /// A time series containing a day for an hourly-read metering point has 24 values, where position 1 is the first hour.
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// The amount of energy
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// The quality of the position's energy quantity, e.g. 'As read', 'Estimated' etc.
        /// </summary>
        public string? Quality { get; set; }

        /// <summary>
        /// Calculated start time for this data point
        /// </summary>
        public DateTime? Time { get; set; }
    }
}
