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

namespace GreenEnergyHub.Messaging.Dispatching
{
    /// <summary>
    /// Dispatch a command
    /// </summary>
    public interface IHubCommandMediator
    {
        /// <summary>
        /// Dispatch a command to a <see cref="HubCommandHandler{TCommand}"/>
        /// </summary>
        /// <param name="command">Command to dispatch</param>
        /// <typeparam name="TCommand">Type of command</typeparam>
        Task DispatchAsync<TCommand>(TCommand command)
            where TCommand : IHubMessage;
    }
}
