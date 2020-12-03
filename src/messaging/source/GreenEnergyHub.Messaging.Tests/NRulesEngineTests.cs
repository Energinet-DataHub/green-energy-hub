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

using System.Threading.Tasks;
using AutoFixture;
using GreenEnergyHub.Messaging.RulesEngine;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests
{
    // https://github.com/NRules/NRules/issues/103
    public class NRulesEngineTests
    {
        private readonly Fixture _fixture;

        public NRulesEngineTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ValidateAsync_SuccessfulRules_ReturnTrueAsync()
        {
            var ruleEngine = new NRulesEngine<StubMessage>(new AlwaysTrueRuleSet());
            var message = _fixture.Create<StubMessage>();
            bool isValid = await ruleEngine.ValidateAsync(message).ConfigureAwait(false);
            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateAsync_UnsuccessfulRules_ReturnFalseAsync()
        {
            var ruleEngine = new NRulesEngine<StubMessage>(new AlwaysFalseRuleSet());
            var message = _fixture.Create<StubMessage>();
            bool isValid = await ruleEngine.ValidateAsync(message).ConfigureAwait(false);
            Assert.False(isValid);
        }

        [Fact]
        public async Task ValidateAsync_MixedSuccessfulRules_ReturnFalseAsync()
        {
            var ruleEngine = new NRulesEngine<StubMessage>(new MixedResultRuleSet());
            var message = _fixture.Create<StubMessage>();
            bool isValid = await ruleEngine.ValidateAsync(message).ConfigureAwait(false);
            Assert.False(isValid);
        }
    }
}
