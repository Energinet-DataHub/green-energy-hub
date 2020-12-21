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
using GreenEnergyHub.Messaging.MessageTypes.Common;
using NodaTime;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// Represents a message in the Green Energy Hub.
    /// </summary>
    public interface IHubMessage
    {
        /// <summary>
        /// A unique id for this request.
        /// </summary>
        /// <value>A string.</value>
        Transaction Transaction { get; set; }

        /// <summary>
        /// The date the message was sent.
        /// </summary>
        /// <value>A DateTime.</value>
        Instant RequestDate { get; set; }
    }
}
