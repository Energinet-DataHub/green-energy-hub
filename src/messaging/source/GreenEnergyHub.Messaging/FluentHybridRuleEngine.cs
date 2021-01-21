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
using FluentValidation;
using GreenEnergyHub.Messaging.Validation;

namespace GreenEnergyHub.Messaging
{
    public sealed class FluentHybridRuleEngine<T> : IRuleEngine<T>
    {
        private readonly RuleCollection<T> _ruleCollection;
        private readonly ServiceProviderDelegate _serviceProviderDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentHybridRuleEngine{T}"/> class.
        /// </summary>
        public FluentHybridRuleEngine(RuleCollection<T> ruleCollection, ServiceProviderDelegate serviceProviderDelegate)
        {
            _ruleCollection = ruleCollection ?? throw new ArgumentNullException(nameof(ruleCollection));
            _serviceProviderDelegate = serviceProviderDelegate ?? throw new ArgumentNullException(nameof(serviceProviderDelegate));
        }

        /// <inheritdoc cref="IRuleEngine{T}"/>
        public async Task<RuleResultCollection> ValidateAsync(T message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var context = new ValidationContext<T>(message);

            return await _ruleCollection.ValidateAsync(context, _serviceProviderDelegate).ConfigureAwait(false);
        }
    }
}
