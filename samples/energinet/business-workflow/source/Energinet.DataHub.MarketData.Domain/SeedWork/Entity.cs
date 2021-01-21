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

namespace Energinet.DataHub.MarketData.Domain.SeedWork
{
    public abstract class Entity
    {
        private List<IDomainEvent>? _domainEvents;

        /// <summary>
        /// Domain events occurred.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();

        /// <summary>
        /// Clears all recorded events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        // /// <summary>
        // /// Validates a bussines rule
        // /// </summary>
        // /// <param name="rule"></param>
        // /// <returns><see cref="BusinessRuleValidationResult"/></returns>
        // /// <exception cref="ArgumentNullException"><see cref="ArgumentNullException"/></exception>
        // protected static BusinessRuleValidationResult CheckRule(IBusinessRule rule)
        // {
        //     if (rule is null)
        //     {
        //         throw new ArgumentNullException(nameof(rule));
        //     }
        //
        //     return rule.Validate();
        // }

        /// <summary>
        /// Add domain event.
        /// </summary>
        /// <param name="domainEvent">Domain event.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();

            _domainEvents.Add(domainEvent);
        }
    }
}
