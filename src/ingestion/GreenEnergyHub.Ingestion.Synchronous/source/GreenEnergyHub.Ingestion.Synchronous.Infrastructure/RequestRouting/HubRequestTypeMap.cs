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
using GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestMediation;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestRouting
{
    /// <summary>
    /// A class which maps the relationship between category and RequestType.
    /// </summary>
    public class HubRequestTypeMap : IHubRequestTypeMap
    {
        private readonly Dictionary<string, Type> _registrations;

        public HubRequestTypeMap(IEnumerable<RequestRegistration> registrations)
        {
            _registrations = registrations.ToDictionary(
                key => key.RequestName,
                val => val.RequestType);
        }

        /// <summary>
        /// Retrieves an IHubActionRequest for the given category if one exists.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>An IHubActionRequest type if it exists: null otherwise.
        /// </returns>
        public Type? GetTypeByCategory(string category)
        {
            return !_registrations.ContainsKey(category) ? null : _registrations[category];
        }
  }
}
