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
using System.Collections;
using System.Collections.Generic;

namespace GreenEnergyHub.Messaging.Validation
{
    /// <summary>
    /// A collection of failed validations
    /// </summary>
    public sealed class RuleResultCollection : IReadOnlyCollection<RuleResult>
    {
        private readonly List<RuleResult> _results = new List<RuleResult>();

        private RuleResultCollection(IEnumerable<RuleResult> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));

            _results.AddRange(results);
        }

        /// <summary>
        /// Number of failed validations
        /// </summary>
        public int Count => _results.Count;

        /// <summary>
        /// <c>true</c> if the validation did not fail
        /// </summary>
        public bool Success => _results.Count == 0;

        /// <inheritdoc cref="IReadOnlyCollection{T}"/>
        public IEnumerator<RuleResult> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        /// <summary>
        /// Create a <see cref="RuleResultCollection"/> from a <see cref="IEnumerable"/> of <see cref="RuleResult"/>
        /// </summary>
        /// <param name="results">Collection of <see cref="RuleResult"/></param>
        /// <returns><see cref="RuleResultCollection"/> containing <paramref name="results"/></returns>
        internal static RuleResultCollection From(IEnumerable<RuleResult> results)
            => new RuleResultCollection(results);
    }
}
