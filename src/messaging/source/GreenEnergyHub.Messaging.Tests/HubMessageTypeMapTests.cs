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
using GreenEnergyHub.Messaging.MessageRouting;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests
{
    public class HubMessageTypeMapTests
    {
        private readonly AutoMocker _autoMocker;

        public HubMessageTypeMapTests()
        {
            _autoMocker = new AutoMocker(MockBehavior.Default);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ValidCategory_ReturnsCorrectType()
        {
            var messageMaps = new[] { new MessageRegistration("ServiceStart", typeof(Type)) };
            var hubMessageTypeMap = new HubMessageTypeMap(messageMaps);

            var result = hubMessageTypeMap.GetTypeByCategory("ServiceStart");
            Assert.Equal(typeof(Type), result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void InvalidCategory_ReturnsNull()
        {
            var messageMaps = Array.Empty<MessageRegistration>();
            var hubMessageTypeMap = new HubMessageTypeMap(messageMaps);

            var result = hubMessageTypeMap.GetTypeByCategory("ServiceStart");
            Assert.Null(result);
        }
    }
}
