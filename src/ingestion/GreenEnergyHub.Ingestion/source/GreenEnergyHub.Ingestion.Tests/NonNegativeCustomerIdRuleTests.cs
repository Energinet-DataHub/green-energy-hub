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
using GreenEnergyHub.Ingestion.RequestTypes.Common;
using GreenEnergyHub.Ingestion.Rules;
using GreenEnergyHub.Ingestion.Tests.TestHelpers;
using NRules;
using NRules.Fluent;
using Xunit;

namespace GreenEnergyHub.Ingestion.Tests
{
    // https://github.com/NRules/NRules/issues/103
    public class NonNegativeCustomerIdRuleTests
    {
        private readonly ISession _session;
        private readonly Fixture _fixture;

        public NonNegativeCustomerIdRuleTests()
        {
            _fixture = new Fixture();

            var repository = new RuleRepository();
            repository.Load(x => x.From(new Type[] { typeof(NonNegativeCustomerIdRule<MockHasCustomerId>) }));
            var factory = repository.Compile();
            _session = factory.CreateSession();
        }

        [Fact]
        public void RulesEngine_FactWithNonNegativeCustomerId_YieldsValidRuleResult()
        {
            const string? customerId = "1";
            var fact = _fixture.Build<MockHasCustomerId>()
                .With(_ => _.Consumer, new MarketParticipant(customerId))
                .Create();

            _session.Insert(fact);
            _session.Fire();
            var results = _session.Query<RuleResult>().ToList();

            Assert.Single(results);
            Assert.True(results[0].IsSuccessful);
        }

        [Fact]
        public void RulesEngine_FactWithNegativeCustomerId_YieldsInvalidRuleResult()
        {
            var customerId = "-1";
            var fact = _fixture.Build<MockHasCustomerId>()
                        .With(_ => _.Consumer, new MarketParticipant(customerId))
                        .Create();

            _session.Insert(fact);
            _session.Fire();
            var results = _session.Query<RuleResult>().ToList();

            Assert.Single(results);
            Assert.False(results[0].IsSuccessful);
        }
    }
}
