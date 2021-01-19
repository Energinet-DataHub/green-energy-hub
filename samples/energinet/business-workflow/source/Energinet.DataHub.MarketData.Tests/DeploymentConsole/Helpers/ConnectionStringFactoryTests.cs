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
using Energinet.DataHub.MarketData.DeploymentConsole.Helpers;
using Xunit;

namespace Energinet.DataHub.MarketData.Tests.DeploymentConsole.Helpers
{
    public class ConnectionStringFactoryTests
    {
        [Fact]
        public void GetConnectionString_WhenNoArgs_ReturnsDefault()
        {
            // Arrange
            var args = System.Array.Empty<string>();

            // Act
            var result = ConnectionStringFactory.GetConnectionString(args);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void GetConnectionString_WhenConnectionStringProvided_StringIsReturned()
        {
            // Arrange
            var connectionString = "SomeString";
            var args = new string[] { connectionString };

            // Act
            var result = ConnectionStringFactory.GetConnectionString(args);

            // Assert
            Assert.Equal(connectionString, result);
        }
    }
}
