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
using System.IO;
using System.Threading.Tasks;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// Rehydrates an object from a stream
    /// </summary>
    public interface IHubRehydrator
    {
        /// <summary>
        /// Rehydrate a message
        /// </summary>
        /// <param name="message"><see cref="Stream"/> containing the message</param>
        /// <param name="messageType">Message type to rehydrate</param>
        /// <returns>If the message type is known and the message content valid a <see cref="IHubMessage"/> else null</returns>
        Task<IHubMessage?> RehydrateAsync(Stream message, Type messageType);

        /// <summary>
        /// Rehydrates a collection of hub requests
        /// </summary>
        /// <param name="message"><see cref="Stream"/> containing the message</param>
        /// <param name="messageType">Message type to rehydrate</param>
        /// <returns>A collection of <see cref="IHubMessage"/></returns>
        Task<IEnumerable<IHubMessage>?> RehydrateCollectionAsync(Stream message, Type messageType);
    }
}
