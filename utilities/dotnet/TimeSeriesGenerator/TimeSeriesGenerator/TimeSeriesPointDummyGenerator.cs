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
using NodaTime;
using TimeSeriesGenerator.Domain;

namespace TimeSeriesGenerator
{
    /// <summary>
    /// Generates dummy data to ensure fairly realistic package size
    /// </summary>
    internal static class TimeSeriesPointDummyGenerator
    {
        public static TimeSeriesPoint Massage(TimeSeriesPoint tsp, Random random, int numberOfMeteringPoints)
        {
            tsp.MeteringGridArea_Domain_mRID = "592";
            // Make the last digits a number from 1 to number of metering points
            tsp.MarketEvaluationPoint_mRID = $"57803299{random.Next(1, numberOfMeteringPoints):D10}";
            tsp.Quality = "D01";
            tsp.QuantityMeasurementUnit_Name = "KWH";
            tsp.MeterReadingPeriodicity = "PT15M";
            tsp.Product = "8716867000030";
            tsp.MeteringMethod = "D01";
            tsp.ConnectionState = "E23";
            tsp.Parent_Domain_mRID = "578032999778756222";
            tsp.ServiceCategoryKind = "Unknown";
            tsp.Technology = "Unknown";
            tsp.TimeSeries_mRID = Guid.NewGuid().ToString();
            tsp.MktActivityRecord_Status = "9";
            tsp.ProcessType = "E30";
            tsp.SenderMarketParticipantMarketRole_Type = "DDQ";
            tsp.MarketDocument_mRID = "HEDmRID101099323243434344343443";
            tsp.CreatedDateTime = SystemClock.Instance.GetCurrentInstant();
            tsp.SenderMarketParticipant_mRID = "1234567890123";
            tsp.MessageReference = "MS10000100101010011";
            tsp.CorrelationId = Guid.NewGuid().ToString();
            return tsp;
        }
    }
}
