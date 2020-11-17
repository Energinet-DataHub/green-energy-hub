﻿// Copyright 2020 Energinet DataHub A/S
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

namespace GreenEnergyHub.Ingestion.RequestRouting
{
    /// <summary>
    /// Holds a mapping from category to IHubRequestType.
    /// </summary>
    public interface IHubRequestTypeMap
    {
        /// <summary>
        /// Uses a mapping to retrieve an IHubRequestType from a given category.
        /// </summary>
        /// <param name="category">The string category.</param>
        /// <returns>An IHubRequestType</returns>
        Type? GetTypeByCategory(string category);
    }
}