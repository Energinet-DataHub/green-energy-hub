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
#nullable disable
using GreenEnergyHub.Messaging.MessageTypes;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace GreenEnergyHub.Messaging.Rules
{
    /// <summary>
    /// Verify if a customerId is non-negative. Note that the non-negative
    /// verification is part of the action body, not the match condition.
    ///
    /// Moving the verification to match condition would not result in a
    /// RuleResult upon validation rule failure.
    /// </summary>
    [Repeatability(RuleRepeatability.NonRepeatable)]
    public class NonNegativeCustomerIdRule<TMessage> : Rule
        where TMessage : IHubMessage, IHubMessageHasConsumer
    {
        /// <summary>
        /// Definition of the NRule.
        /// </summary>
        public override void Define()
        {
            TMessage message = default;

            When()
                .Match<TMessage>(() => message);
            Then()
                .Yield(_ => DoValidation(message));
        }

        private RuleResult DoValidation(TMessage message)
        {
            if (int.TryParse(message.Consumer.MRID.Value, out var mrid) && mrid < 0)
            {
                return new RuleResult(GetType().Name, message.Transaction.MRID, false, "CustomerId was negative");
            }

            return new RuleResult(GetType().Name, message.Transaction.MRID, true);
        }
    }
}
#nullable restore
