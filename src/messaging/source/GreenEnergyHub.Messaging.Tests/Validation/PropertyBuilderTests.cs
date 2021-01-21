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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using GreenEnergyHub.Messaging.Tests.TestHelpers.Validation;
using GreenEnergyHub.Messaging.Validation;
using Xunit;
using ChangeOfSupplier = GreenEnergyHub.Messaging.Tests.TestHelpers.ChangeOfSupplier;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    [Trait("Category", "Unit")]
    public class PropertyBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void PropertyRuleShouldBeAssigned()
        {
            // Arrange
            Expression<Func<ChangeOfSupplier, string?>> selector = cos => cos.MarketEvaluationPoint;
            var tracking = new List<Action<ServiceProviderDelegate, AbstractValidator<ChangeOfSupplier>>>();

            var builder = new PropertyBuilder<ChangeOfSupplier, string?>(
                selector,
                tracking);

            // Act
            builder.PropertyRule<MarketEvaluationPointValidation>();

            // Assert
            tracking.Should().HaveCount(1);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RuleCollectionShouldBeAssigned()
        {
            // Arrange
            Expression<Func<MarketParticipant, string>> selector = mp => mp.Name!;
            var tracking = new List<Action<ServiceProviderDelegate, AbstractValidator<MarketParticipant>>>();

            var builder = new PropertyBuilder<MarketParticipant, string>(selector, tracking);

            // Act
            builder.RuleCollection<MarketParticipantNameRuleCollection>();

            // Assert
            tracking.Should().HaveCount(1);
        }
    }
}
