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

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// This provides an common interface for validation of messages of type
    /// TMessage that must be implemented for each concrete IRulesEngine
    /// implementation with the required logic to trigger that rule engine's execution.
    /// </summary>
    public interface IRuleEngine<in TMessage>
        where TMessage : IHubMessage
    {
        /// <summary>
        /// Validates the provided message asynchronously.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>True if the message is valid according to the rules in this
        /// IRuleEngine.</returns>
        Task<bool> ValidateAsync(TMessage message);
    }
}
