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
using FluentValidation;

namespace GreenEnergyHub.Messaging.Validation
{
    /// <summary>
    /// A <see cref="RuleCollection{T}"/> serves as the root for input validation.
    /// This class configures the rules for properties on <typeparam name="T"></typeparam>.
    /// <example>
    /// This sample shows how to specify rules for a DTO
    /// <code>
    /// class ChangeSupplierValidation : RuleCollection&lt;ChangeSupplier&gt; {
    ///     public ChangeSupplierValidation() {
    ///         RuleFor(p => p.MarketEvaluationPointMrid)
    ///             .Use&lt;MarketEvaluationPointMrid&gt;();
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class RuleCollection<T>
    {
        private readonly List<Action<ServiceProviderDelegate, AbstractValidator<T>>> _tracking = new List<Action<ServiceProviderDelegate, AbstractValidator<T>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection{T}"/> class.
        /// </summary>
        protected RuleCollection()
        {
        }

        /// <summary>
        /// Validates the <see cref="ValidationContext{T}"/> asynchronous
        /// </summary>
        /// <param name="context">instance to validate</param>
        /// <param name="serviceProviderDelegate">the delegate to resolve instances from</param>
        /// <returns>
        /// Returns a <see cref="RuleResultCollection"/> indicating whether or not the validation was a success.
        /// If the validation was unsuccessful, the <see cref="RuleResultCollection"/> contains a list of error messages.
        /// </returns>
        /// <exception cref="ArgumentNullException">if <paramref name="context"/> or <paramref name="serviceProviderDelegate"/> is null</exception>
        internal async Task<RuleResultCollection> ValidateAsync(ValidationContext<T> context, ServiceProviderDelegate serviceProviderDelegate)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (serviceProviderDelegate == null) throw new ArgumentNullException(nameof(serviceProviderDelegate));

            var validator = GetValidator(serviceProviderDelegate);
            var result = await validator.ValidateAsync(context).ConfigureAwait(false);

            return RuleResultCollection.From(result.Errors.Select(r => new RuleResult(r.ErrorCode, r.ErrorMessage)));
        }

        /// <summary>
        /// Gets the current validator
        /// </summary>
        internal AbstractValidator<T> GetValidator(ServiceProviderDelegate sp)
        {
            if (sp == null) throw new ArgumentNullException(nameof(sp));
            var validator = new InlineValidator<T>();
            _tracking.ForEach(cfg => cfg.Invoke(sp, validator));
            return validator;
        }

        /// <summary>
        /// Defines a validation rule for a property
        /// </summary>
        /// <param name="selector">Property selection</param>
        /// <typeparam name="TProperty">The type of property being validated</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="selector"/> is null.</exception>
        protected PropertyBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new PropertyBuilder<T, TProperty>(selector, _tracking);
        }

        /// <summary>
        /// Defines a validation rule for a property collection
        /// </summary>
        /// <param name="selector">Property collection selection</param>
        /// <typeparam name="TProperty">The type of property being validated</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="selector"/> is null.</exception>
        protected PropertyCollectionBuilder<T, TProperty> RuleForEach<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new PropertyCollectionBuilder<T, TProperty>(selector, _tracking);
        }
    }
}
