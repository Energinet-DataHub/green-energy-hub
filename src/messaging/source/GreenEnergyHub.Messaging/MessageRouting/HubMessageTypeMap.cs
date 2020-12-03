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
using System.Linq;

namespace GreenEnergyHub.Messaging.MessageRouting
{
    /// <summary>
    /// A class which maps the relationship between category and MessageType.
    /// </summary>
    public class HubMessageTypeMap : IHubMessageTypeMap
    {
        private readonly Dictionary<string, Type> _registrations;

        public HubMessageTypeMap(IEnumerable<MessageRegistration> registrations)
        {
            _registrations = registrations.ToDictionary(
                key => key.MessageName,
                val => val.MessageType);
        }

        /// <summary>
        /// Retrieves an IHubMessage for the given category if one exists.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>An IHubMessage type if it exists: null otherwise.
        /// </returns>
        public Type? GetTypeByCategory(string category)
        {
            return !_registrations.ContainsKey(category) ? null : _registrations[category];
        }
  }
}
