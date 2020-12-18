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

using GreenEnergyHub.Messaging;

namespace GreenEnergyHub.Queues
{
    /// <summary>
    /// Factory for creating message queue dispatcher messages
    /// </summary>
    public interface IMessageEnvelopeFactory
    {
        /// <summary>
        /// Creates message envelope for <see cref="IHubMessage"/>
        /// </summary>
        /// <param name="hubMessage"></param>
        /// <returns><see cref="MessageEnvelope"/></returns>
        MessageEnvelope CreateFrom(IHubMessage hubMessage);
    }
}
