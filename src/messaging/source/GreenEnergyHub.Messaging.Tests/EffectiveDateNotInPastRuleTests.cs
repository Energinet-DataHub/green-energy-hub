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
using System.Linq;
using AutoFixture;
using GreenEnergyHub.Messaging.Rules;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using Moq;
using Moq.AutoMock;
using NRules;
using NRules.Fluent;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests
{
    // https://github.com/NRules/NRules/issues/103
    public class EffectiveDateNotInPastRuleTests
    {
        private readonly ISession _session;
        private readonly AutoMocker _autoMocker;
        private readonly Fixture _fixture;

        public EffectiveDateNotInPastRuleTests()
        {
            _fixture = new Fixture();
            _autoMocker = new AutoMocker(MockBehavior.Default);

            var repository = new RuleRepository();
            repository.Load(x => x.From(new Type[] { typeof(EffectiveDateNotInPastRule<MockHasStartDate>) }));
            var factory = repository.Compile();
            _session = factory.CreateSession();
        }

        [Fact]
        public void RulesEngine_FactWithPastDate_YieldsInvalidRuleResult()
        {
            var pastDate = DateTime.UtcNow - TimeSpan.FromDays(1);
            var fact = _fixture.Build<MockHasStartDate>()
                .With(_ => _.StartDate, pastDate)
                .Create();

            _session.Insert(fact);
            _session.Fire();
            var results = _session.Query<RuleResult>().ToList();

            Assert.Single(results);
            Assert.False(results[0].IsSuccessful);
        }

        [Fact]
        public void RulesEngine_FactWithFutureDate_YieldsValidRuleResult()
        {
            var futureDate = DateTime.UtcNow + TimeSpan.FromDays(1);
            var fact = _fixture.Build<MockHasStartDate>()
                        .With(_ => _.StartDate, futureDate)
                        .Create();

            _session.Insert(fact);
            _session.Fire();
            var results = _session.Query<RuleResult>().ToList();

            Assert.Single(results);
            Assert.True(results[0].IsSuccessful);
        }
    }
}
