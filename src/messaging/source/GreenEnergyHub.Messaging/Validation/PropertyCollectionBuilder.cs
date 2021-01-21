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
    public class PropertyCollectionBuilder<T, TProperty>
    {
        private readonly Expression<Func<T, IEnumerable<TProperty>>> _selectorForEach;
        private readonly List<Action<ServiceProviderDelegate, AbstractValidator<T>>> _tracking;

        internal PropertyCollectionBuilder(
            Expression<Func<T, IEnumerable<TProperty>>> selector,
            List<Action<ServiceProviderDelegate, AbstractValidator<T>>> tracking)
        {
            _selectorForEach = selector ?? throw new ArgumentNullException(nameof(selector));
            _tracking = tracking;
        }

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
