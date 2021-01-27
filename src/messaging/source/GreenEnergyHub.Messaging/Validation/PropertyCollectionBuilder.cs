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
    /// <summary>
    /// A builder for properties that implement <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">class that contains the property</typeparam>
    /// <typeparam name="TProperty">property type to validate</typeparam>
    public class PropertyCollectionBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, IEnumerable<TProperty>>> _selectorForEach;
        private readonly List<Action<ServiceProviderDelegate, AbstractValidator<T>>> _tracking;

        /// <summary>
        /// Creates a new builder
        /// </summary>
        /// <param name="selector">selector for the property</param>
        /// <param name="tracking">a list of rules to apply to the current validator</param>
        internal PropertyCollectionBuilder(
            Expression<Func<T, IEnumerable<TProperty>>> selector,
            List<Action<ServiceProviderDelegate, AbstractValidator<T>>> tracking)
        {
            _selectorForEach = selector ?? throw new ArgumentNullException(nameof(selector));
            _tracking = tracking;
        }

        /// <summary>
        /// Assign a validator to the selector
        /// </summary>
        /// <typeparam name="TValidator">Validator to assign</typeparam>
        /// <exception cref="InvalidOperationException">If the <see cref="PropertyRule{TValidator}"/> is not registered within the service provider</exception>
        public PropertyCollectionBuilder<T, TProperty> PropertyRule<TValidator>()
            where TValidator : PropertyRule<TProperty>
        {
            _tracking.Add((sp, validator) =>
            {
                if (!(sp.Invoke(typeof(TValidator)) is TValidator rule)) throw new InvalidOperationException();
                validator.RuleForEach(_selectorForEach).SetValidator(rule).WithErrorCode(rule.Code);
            });

            return this;
        }

        /// <summary>
        /// Assign a validator to the selector
        /// </summary>
        /// <typeparam name="TCollection">Validator to assign for the collection</typeparam>
        /// <exception cref="InvalidOperationException">If the <see cref="PropertyRule{TValidator}"/> is not registered within the service provider</exception>
        public PropertyCollectionBuilder<T, TProperty> RuleCollection<TCollection>()
            where TCollection : RuleCollection<TProperty>
        {
            _tracking.Add((sp, validator) =>
            {
                if (!(sp.Invoke(typeof(TCollection)) is TCollection rule)) throw new InvalidOperationException();
                validator.RuleForEach(_selectorForEach).SetValidator(rule.GetValidator(sp));
            });
            return this;
        }
    }
}
