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
using Energinet.DataHub.SoapAdapter.Application.Converters.Iso8601;
using FluentAssertions;
using GreenEnergyHub.TestHelpers.Traits;
using NodaTime;
using Xunit;

namespace Energinet.DataHub.SoapAdapter.Tests.Iso8601
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class Iso8601DurationTests
    {
        [Theory]
        [InlineData(true, "PT1H")]
        [InlineData(false, null)]
        public void GetObservationTime_WhenStartTimeOrResolutionIsNull_ShouldThrow(bool startTimeIsNull, string resolutionDuration)
        {
            // Arrange
            var startTime = startTimeIsNull ? null as Instant? : Instant.FromUtc(2020, 1, 1, 1, 1);
            var position = 1;

            // Act
            Action act = () => Iso8601Duration.GetObservationTime(startTime, resolutionDuration, position);

            // Assert
            Assert.Throws<NullReferenceException>(act);
        }

        [Theory]
        [InlineData("PT1H", 4, 8, 11, 0)]
        [InlineData("PT15M", 6, 10, 11, 15)]
        public void GetObservationTime_WhenCalledWithCertainPosition_ShouldGiveCorrectTime(string resolutionDuration, int position, int startHour, int expectedHour, int expectedMinute)
        {
            // Arrange
            var startTime = Instant.FromUtc(2020, 1, 1, startHour, 0);

            // Act
            var instant = Iso8601Duration.GetObservationTime(startTime, resolutionDuration, position);

            // Assert
            var result = instant.InUtc();
            result.Hour.Should().Be(expectedHour);
            result.Minute.Should().Be(expectedMinute);
        }
    }
}
