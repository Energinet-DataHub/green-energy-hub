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
using GreenEnergyHub.Messaging.RequestTypes.Common;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// Represents a request to take some action in the Green Energy Hub,
    /// expecting an IHubActionResponse back.
    /// </summary>
    public interface IHubRequest
    {
        /// <summary>
        /// A unique id for this request.
        /// </summary>
        /// <value>A string.</value>
        Transaction Transaction { get; set; }

        /// <summary>
        /// The date of the request.
        /// </summary>
        /// <value>A DateTime.</value>
        DateTime RequestDate { get; set; }
    }
}
