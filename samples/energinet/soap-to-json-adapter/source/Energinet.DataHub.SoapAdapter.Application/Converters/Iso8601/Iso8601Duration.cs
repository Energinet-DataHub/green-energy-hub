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

namespace Energinet.DataHub.SoapAdapter.Application.Converters.Iso8601
{
    public static class Iso8601Duration
    {
        public static Instant GetObservationTime(Instant? startTime, string? resolutionDuration, int position)
        {
            if (startTime == null || string.IsNullOrEmpty(resolutionDuration))
            {
                throw new NullReferenceException();
            }

            var index = position - 1;

            return resolutionDuration switch
            {
                "PT1H" => startTime.Value.Plus(Duration.FromHours(index)),
                "PT15M" => startTime.Value.Plus(Duration.FromMinutes(15 * index)),
                _ => throw new ArgumentException($"Unknown time resolution: {resolutionDuration}"),

                /* "P1M" is expected to be implemented in the future. Be aware that is is not enough to just add a number of months.
                 First: We need to convert from UTC to local time. If the start time is midnight the 1.st local time then the start time
                 may be late the 30. in the preceding month in UTC if that month only have 30 days. Adding 1 month may then result
                 in a time on day 30 in a month with 31 days instead of the last day for than month.
                 Second: A time series may start or end in the middle of a month.
                 - If the metering point is connected during the month - then the time series will start from the connection date.
                 - If metering point is closed down during the month - Then the time series will end at the closed down date.
                 Basically, something simple like below, won't satisfy our needs,
                 "P1M" => startTime.ToLocalTime().AddMonths(numberOfDurations).ToUniversalTime() */
            };
        }
    }
}
