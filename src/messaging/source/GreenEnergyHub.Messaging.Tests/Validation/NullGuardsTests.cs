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
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using GreenEnergyHub.Messaging.Validation;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    [Trait("Category", "Unit")]
    public class NullGuardsTests
    {
        [Fact]
        public void FluentHybridRuleEngineConstructor_Should_HaveNullGuard()
        {
            var fixture = new Fixture();
            fixture.Register<RuleCollection<ChangeOfSupplier>>(() => new ChangeOfSupplierRuleCollection());
            var assertion = new GuardClauseAssertion(fixture);

            assertion.Verify(typeof(FluentHybridRuleEngine<ChangeOfSupplier>).GetConstructors());
        }

        [Fact]
        public async Task FluentHybridRuleEngineValidate_Should_HaveNullGuard()
        {
            var fixture = new Fixture();
            fixture.Register<RuleCollection<ChangeOfSupplier>>(() => new ChangeOfSupplierRuleCollection());

            var sut = fixture.Create<FluentHybridRuleEngine<ChangeOfSupplier>>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await
                    sut.ValidateAsync(null!).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [Fact]
        public void PropertyBuilder_Should_HaveNullGuard()
        {
            var fixture = new Fixture();
            var assertion = new GuardClauseAssertion(fixture);

            assertion.Verify(typeof(PropertyBuilder<ChangeOfSupplier, string?>).GetConstructors());
        }

        [Fact]
        public void PropertyCollectionBuilder_Should_HaveNullGuard()
        {
            var fixture = new Fixture();
            var assertion = new GuardClauseAssertion(fixture);

            assertion.Verify(typeof(PropertyCollectionBuilder<ChangeOfSupplier, string?>).GetConstructors(BindingFlags.NonPublic));
        }
    }
}
