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

using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints;
using Xunit;

namespace Energinet.DataHub.MarketData.Tests.Domain.MarketEvaluationPoints
{
    public class MeteringPointIdTests
    {
        [Theory]
        [InlineData("571234567891234368")]
        public void Should_throw_when_checksum_digit_is_invalid(string gsrnValue)
        {
            Assert.Throws<InvalidMeteringPointIdRuleException>(() => MarketEvaluationPointMrid.Create(gsrnValue));
        }

        [Theory]
        [InlineData("111234567891234368")]
        public void Should_throw_when_start_digits_are_invalid(string gsrnValue)
        {
            Assert.Throws<InvalidMeteringPointIdRuleException>(() => MarketEvaluationPointMrid.Create(gsrnValue));
        }

        [Theory]
        [InlineData("57123456789123456")]
        public void Should_throw_when_length_is_invalid(string gsrnValue)
        {
            Assert.Throws<InvalidMeteringPointIdRuleException>(() => MarketEvaluationPointMrid.Create(gsrnValue));
        }

        [Theory]
        [InlineData("571234567891234568")]
        public void Should_create_when_gsrn_value_is_valid(string gsrnValue)
        {
            Assert.NotNull(MarketEvaluationPointMrid.Create(gsrnValue));
        }
    }
}
