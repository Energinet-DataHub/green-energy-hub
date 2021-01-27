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
using FluentValidation.Validators;

namespace GreenEnergyHub.Messaging.Validation
{
    /// <summary>
    /// The class is used to validate a single property
    /// </summary>
    /// <typeparam name="T">PropertyType to validate</typeparam>
    public abstract class PropertyRule<T> : PropertyRule
    {
        /// <summary>
        /// Validates the property - this is the contract from FluentValidation
        /// </summary>
        /// <param name="context">context describing what is being validated</param>
        /// <returns><c>true</c> if successful, else <c>false</c></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="context"/> is null</exception>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.PropertyValue is T propertyValue) return IsValid(propertyValue, context);

            return false;
        }

        /// <summary>
        /// Validates the property value
        /// </summary>
        /// <param name="propertyValue">Value to validate</param>
        /// <param name="context">The context for the property</param>
        /// <returns><c>true</c> if successful, else <c>false</c></returns>
        protected abstract bool IsValid(T propertyValue, PropertyValidatorContext context);
    }
}
