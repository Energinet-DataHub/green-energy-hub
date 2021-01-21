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
using System.Threading.Tasks;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using GreenEnergyHub.Messaging.Validation;
using Moq;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests
{
    public class FluentHybridRuleEngineTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Null_Message_should_raise_null_guard()
        {
            var ruleCollectionMock = new Mock<RuleCollection<TestMessage>>();
            var sp = new Mock<ServiceProviderDelegate>();
            var sut = new FluentHybridRuleEngine<TestMessage>(ruleCollectionMock.Object, sp.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await sut.ValidateAsync(null!).ConfigureAwait(false))
                .ConfigureAwait(false);
        }
    }
}
