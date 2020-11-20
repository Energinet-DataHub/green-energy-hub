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
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using Xunit;

namespace ValidatorToolTests
{
    public class FluentValidationTests
    {
        private readonly IRuleEngine _ruleEngine;

        public FluentValidationTests()
        {
            _ruleEngine = new FluentValidationEngine();
        }

        [Fact]
        public async Task NonNegativeValueAndValidCustomerId_ReturnsTrue()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 1);
            Assert.True(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NonNegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NegativeValueAndValidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 3);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }
    }
}
