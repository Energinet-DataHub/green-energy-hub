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
using GreenEnergyHub.Ingestion.Synchronous.Application;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RulesEngine
{
    /// <summary>
    /// An IEnumerable of rules which apply to the parameterized request type.
    /// </summary>
    /// <typeparam name="TRequest">The request type which this ruleset applies
    /// to.</typeparam>
    public interface IHubRuleSet<out TRequest>
        where TRequest : IHubActionRequest
    {
        /// <summary>
        /// The IEnumerable of rules.
        /// </summary>
        /// <value>The rules.</value>
        IEnumerable<Type> Rules { get; }
    }
}
