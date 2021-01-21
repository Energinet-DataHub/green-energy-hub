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

using System.Threading.Tasks;
using GreenEnergyHub.Messaging.Validation;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// This provides an common interface for validation of messages of type
    /// TMessage that must be implemented for each concrete IRulesEngine
    /// implementation with the required logic to trigger that rule engine's execution.
    /// </summary>
    public interface IRuleEngine<in TMessage>
    {
        /// <summary>
        /// Validates the provided message asynchronously.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>
        /// Returns a <see cref="RuleResultCollection"/> indicating whether or not the validation was a success.
        /// If the validation was unsuccessful, the <see cref="RuleResultCollection"/> contains a list of error messages.
        /// </returns>
        Task<RuleResultCollection> ValidateAsync(TMessage message);
    }
}
