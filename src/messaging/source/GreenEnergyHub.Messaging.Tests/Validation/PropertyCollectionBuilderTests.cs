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
using System.Threading.Tasks;
using FluentValidation;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    public class PropertyCollectionBuilderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task PropertyCollectionBuilder_PropertyRule()
        {
            var cos = new ChangeOfSupplier(new List<int>());
            cos.ListOfNumbers.Add(2);
            cos.ListOfNumbers.Add(5);
            cos.ListOfNumbers.Add(12);
            var ruleset = new ChangeOfSupplierRuleCollection();

            var result = await ruleset.ValidateAsync(new ValidationContext<ChangeOfSupplier>(cos), SP).ConfigureAwait(false);
            Assert.False(result.Success);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task PropertyCollectionBuilder_RuleCollection()
        {
            var mp = new MarketParticipant("216asdAdsasd", "This is a Test");
            mp.List.Add("This is an item.");
            mp.List.Add("This string has more than 5 characters.");
            var ruleset = new MarketParticipantListRuleCollection();

            var result = await ruleset.ValidateAsync(new ValidationContext<MarketParticipant>(mp), SP)
                .ConfigureAwait(false);
            Assert.True(result.Success);
        }

        private static object SP(Type type)
        {
            return Activator.CreateInstance(type) ?? throw new InvalidOperationException("Type not found");
        }
    }
}
