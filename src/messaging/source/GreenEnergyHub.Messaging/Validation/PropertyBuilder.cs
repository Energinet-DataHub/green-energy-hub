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
using System.Linq.Expressions;
using FluentValidation;

namespace GreenEnergyHub.Messaging.Validation
{
    public class PropertyBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, TProperty>> _selector;
        private readonly List<Action<ServiceProviderDelegate, AbstractValidator<T>>> _tracking;

        /// <summary>
        /// Creates a new builder
        /// </summary>
        /// <param name="selector">Property selector</param>
        /// <param name="tracking">Tracking list of validators to apply to the selector</param>
        internal PropertyBuilder(
            Expression<Func<T, TProperty>> selector,
            List<Action<ServiceProviderDelegate, AbstractValidator<T>>> tracking)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _tracking = tracking;
        }

        /// <summary>
        /// Assign a rule to a property.
        /// </summary>
        /// <typeparam name="TValidator">The type of validator to assign to a given property</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if the type of the <typeparamref name="TValidator"/> is not found in the service provider.</exception>
        public PropertyBuilder<T, TProperty> PropertyRule<TValidator>()
            where TValidator : PropertyRule<TProperty>
        {
            _tracking.Add((sp, validator) =>
            {
                if (!(sp.Invoke(typeof(TValidator)) is TValidator rule)) throw new InvalidOperationException();
                validator.RuleFor(_selector).SetValidator(rule).WithErrorCode(rule.Code);
            });

            return this;
        }

        /// <summary>
        /// Assign a rule to a property
        /// </summary>
        /// <typeparam name="TCollection">Collection of rules to assign</typeparam>
        /// <exception cref="InvalidOperationException">If the validator is not registered within the service provider</exception>
        public PropertyBuilder<T, TProperty> RuleCollection<TCollection>()
            where TCollection : RuleCollection<TProperty>
        {
            _tracking.Add((sp, validator) =>
            {
                if (!(sp.Invoke(typeof(TCollection)) is TCollection rule)) throw new InvalidOperationException();
                validator.RuleFor(_selector).SetValidator(rule.GetValidator(sp));
            });

            return this;
        }
    }
}
