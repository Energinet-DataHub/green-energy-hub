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
using GreenEnergyHub.Messaging.Dispatching;
using GreenEnergyHub.Messaging.MessageRouting;
using GreenEnergyHub.Messaging.Validation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Messaging.Integration.ServiceCollection
{
    /// <summary>
    /// Static class which allows the Azure Functions runtime IServiceCollection
    /// to discover and register classes needed to run the Green Energy Hub
    /// project.
    /// </summary>
    public static class HandlerExtensions
    {
        /// <summary>
        /// Searches the provided assemblies, as well as the common assemblies,
        /// to find the HubRequestHandlers, HubCommandHandlers, IHubMessages,
        /// IRuleEngines, the rules (represented as a list of
        /// Types), the IEndpointResolver, and the IHubMessageTypeMap, and to
        /// automatically register and provide this instances on function
        /// startup.
        /// </summary>
        /// <param name="services">The IServiceCollection services provided by
        /// the Azure Functions runtime to register our interfaces with.</param>
        /// <param name="customerAssemblies">A list of assemblies to search for
        /// interfaces to automatically register.</param>
        public static void AddGreenEnergyHub(this IServiceCollection services, params System.Reflection.Assembly[] customerAssemblies)
        {
            // Register framework-provided classes last
            var assemblies = customerAssemblies.Append(typeof(IHubMessage).Assembly).ToArray();

            services.AddMediatR(assemblies);

            // Collection of IHubMessage types to discover related classes for
            var messageTypesToRegister = new List<Type>();

            // Walk assemblies in order passed, discovering unique (by name)
            // declared message types
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();

                // Discover message types but omit any already pending
                // registration by same name from another assembly
                var messageTypes = assemblyTypes
                    .Where(type =>
                        type.GetInterfaces().Contains(typeof(IHubMessage)));
                var newMessageTypes = messageTypes
                    .Where(existingMessageType => messageTypesToRegister
                        .All(messageType => messageType.Name != existingMessageType.Name));
                messageTypesToRegister.AddRange(newMessageTypes);
            }

            foreach (var messageType in messageTypesToRegister)
            {
                // Register with mapping class for the message type
                services.AddTransient(_ => new MessageRegistration(messageType));
            }

            services.AddSingleton<IHubRequestMediator, HubRequestMediator>();
            services.AddSingleton<IHubCommandMediator, HubCommandMediator>();
            services.AddSingleton<IHubMessageTypeMap, HubMessageTypeMap>();

            services.DiscoverValidation(assemblies);
        }
    }
}
