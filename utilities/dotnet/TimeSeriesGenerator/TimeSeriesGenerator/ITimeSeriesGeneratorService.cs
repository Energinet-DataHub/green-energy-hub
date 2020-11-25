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
using TimeSeriesGenerator.Domain;

namespace TimeSeriesGenerator
{
    public interface ITimeSeriesGeneratorService
    {
        List<DateTime> GenerateTimeSpans(DateTime startDate, DateTime endTime, int resolution);

        TimeSeriesPoint[] GenerateTimeSeriesFromCsvFile(
            string csvFile,
        List<DateTime> generatedTimeSpanSet,
            int numberOfMeteringPoints);

        /// <summary>
        /// Generates an exchange timeseries dataset.
        /// </summary>
        /// <param name="generatedTimeSpanSet">The time spans.</param>
        /// <param name="numberOfMeteringPoints">the approximate number of metering points per grid area.</param>
        /// <returns>An array with the time series points</returns>
        TimeSeriesPoint[] ExchangeDataset(
            List<DateTime> generatedTimeSpanSet,
            int numberOfMeteringPoints);

        /// <summary>
        /// Generates a production timeseries dataset.
        /// </summary>
        /// <param name="generatedTimeSpanSet">The time spans.</param>
        /// <param name="numberOfMeteringPoints">The approximate number of metering points per grid area.</param>
        /// <returns>An array with the time series points.</returns>
        TimeSeriesPoint[] ProductionDataset(
            List<DateTime> generatedTimeSpanSet,
            int numberOfMeteringPoints);

        /// <summary>
        /// Generates a consumption timeseries dataset.
        /// </summary>
        /// <param name="generatedTimeSpanSet">The time spans.</param>
        /// <param name="numberOfGridAreas">The number of grid areas</param>
        /// <param name="meteringPointsPerGridArea">The approximate number of metering points per grid area.</param>
        /// <param name="numberOfMeteringPoints">Total number of metering points</param>
        /// <returns>An array with the time series points.</returns>
        TimeSeriesPoint[] ConsumptionDataset(
            List<DateTime> generatedTimeSpanSet,
            int numberOfGridAreas,
            int meteringPointsPerGridArea,
            int numberOfMeteringPoints);
    }
}
