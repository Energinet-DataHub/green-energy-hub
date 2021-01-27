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
using System.Collections;
using System.Collections.Generic;
using GreenEnergyHub.Messaging.Validation;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    [Trait("Category", "Unit")]
    public class RuleResultCollectionTests
    {
        private readonly RuleResult _result1 = new RuleResult("VAR001", "This is an error");
        private readonly RuleResult _result2 = new RuleResult("VAR002", "Oh look another error");

        [Fact]
        public void Result_Should_Be_Valid_When_Empty()
        {
            var sut = RuleResultCollection.From(new List<RuleResult>());

            Assert.True(sut.Success);
        }

        [Fact]
        public void Should_Be_Able_To_Add_Results_To_Collection()
        {
            var sut = RuleResultCollection.From(new List<RuleResult> { _result1, _result2 });

            Assert.Equal(2, sut.Count);
        }

        [Fact]
        public void Result_Should_Fail_When_Containing_Results()
        {
            var sut = RuleResultCollection.From(new List<RuleResult> { _result1 });

            Assert.False(sut.Success);
        }

        [Fact]
        public void Null_Collection_Should_Throw_Argument_Null_Exception()
        {
            IEnumerable<RuleResult> nullResults = null!;
            Assert.Throws<ArgumentNullException>(() => RuleResultCollection.From(nullResults));
        }

        [Fact]
        public void Collection_Should_Implement_IEnumerable()
        {
            IEnumerable sut = RuleResultCollection.From(new[] { _result1, _result2 });

            Assert.NotNull(sut.GetEnumerator());
        }

        [Fact]
        public void Result_Should_Implement_Generic_IEnumerable()
        {
            var sut = RuleResultCollection.From(new[] { _result1, _result2 });

            foreach (var result in sut)
            {
                Assert.NotNull(result);
            }
        }

        [Fact]
        public void Result_Should_Implement_GetEnumerator()
        {
            var sut = RuleResultCollection.From(new[] { _result1, _result2 });

            var enumerator = sut.GetEnumerator();

            Assert.NotNull(enumerator);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(_result1, enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Equal(_result2, enumerator.Current);
        }
    }
}
