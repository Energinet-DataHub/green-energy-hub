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

using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace ValidatorTool.RuleEngines.NRules.Rules
{
    /// <summary>
    /// Verify if a meter value is non-negative. Note that the non-negative
    /// verification is part of the action body, not the match condition.
    ///
    /// Moving the verification to match condition would not result in a
    /// RuleResult upon validation rule failure.
    /// </summary>
    [Repeatability(RuleRepeatability.NonRepeatable)]
    public class NonNegativeMeterValueRule : Rule
    {
        public override void Define()
        {
            // ValidationResult result = null;
            MeterMessage message = null;

            When()
                .Match<MeterMessage>(() => message);
            Then()
                .Yield(_ => DoValidation(message));
        }

        private RuleResult DoValidation(MeterMessage message)
        {
            if (message.MeterValue < 0)
            {
                return new RuleResult(GetType().Name, message.TransactionId, false, "Meter value was negative");
            }

            return new RuleResult(GetType().Name, message.TransactionId, true);
        }
    }
}
