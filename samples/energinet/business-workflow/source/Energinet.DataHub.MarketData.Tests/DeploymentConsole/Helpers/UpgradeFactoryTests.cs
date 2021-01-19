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
using Energinet.DataHub.MarketData.DeploymentConsole.Helpers;
using Xunit;

namespace Energinet.DataHub.MarketData.Tests.DeploymentConsole.Helpers
{
    public class UpgradeFactoryTests
    {
        [Fact]
        public void GetUpgradeEngine_WhenConnectionStringIsEmpty_ThrowsException()
        {
            // Arrange
            var connectionString = string.Empty;

            // Act / Assert
            Assert.Throws<ArgumentException>(() => UpgradeFactory.GetUpgradeEngine(connectionString));
        }

        [Fact]
        public void GetUpgradeEngine_WhenConnectionStringOnlyHasWhiteSpace_ThrowsException()
        {
            // Arrange
            var connectionString = " ";

            // Act / Assert
            Assert.Throws<ArgumentException>(() => UpgradeFactory.GetUpgradeEngine(connectionString));
        }

        [Fact]
        public void GetUpgradeEngine_WhenConnectionStringHasValue_ReturnsEngine()
        {
            // Arrange
            var connectionString = "Something";

            // Act
            var result = UpgradeFactory.GetUpgradeEngine(connectionString);

            // Assert
            Assert.NotNull(result);
        }
    }
}
