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
using Energinet.DataHub.SoapAdapter.Application.Dtos;

namespace Energinet.DataHub.SoapAdapter.Application
{
    /// <summary>
    /// Creates an error message stream wrapped in a <see cref="Response"/>
    /// </summary>
    public interface IErrorResponseFactory
    {
        /// <summary>
        /// Creates an error message stream wrapped in a <see cref="Response"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="code">Soap fault code</param>
        /// <returns>An error message stream wrapped in a <see cref="Response"/></returns>
        Task<Response> CreateAsync(string message, string code = "Client");
    }
}
