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
using GreenEnergyHub.Messaging.RulesEngine;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        /// to find the IHubActionHandlers, IHubMessages, IHubRuleSets,
        /// IEndpoints, IRuleEngines, the rules (represented as a list of
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

            // Collection of IRuleSet types to discover related classes for
            var ruleSetsToRegister = new List<Type>();

            // With the set of unique message types determined, discover
            // rulesets based on assembly ordering
            foreach (var messageType in messageTypesToRegister)
            {
                foreach (var assembly in assemblies)
                {
                    var assemblyTypes = assembly.GetTypes();
                    var genericType = typeof(IHubRuleSet<>).MakeGenericType(messageType);

                    // Discover rule sets but omit any already pending registration by same name from another assembly
                    var ruleSet = assemblyTypes
                        .Where(type => type.GetInterfaces().Contains(genericType))
                        .Where(existingRuleSet => ruleSetsToRegister.All(ruleSetType => ruleSetType.Name != existingRuleSet.Name))
                        .FirstOrDefault();

                    if (ruleSet != default)
                    {
                        // Keep track of the registered rulesets to avoid duplicated registrations in next iterations
                        ruleSetsToRegister.Add(ruleSet);

                        // Register the discovered ruleset
                        services.AddScoped(genericType, ruleSet);
                    }
                }
            }

            foreach (var messageType in messageTypesToRegister)
            {
                // Register with mapping class for the message type
                services.AddTransient(_ => new MessageRegistration(messageType));

                // Register supporting classes
                services.AddScoped(typeof(IRuleEngine<>).MakeGenericType(messageType), typeof(NRulesEngine<>).MakeGenericType(messageType));
            }

            services.AddSingleton<IHubRequestMediator, HubRequestMediator>();
            services.AddSingleton<IHubCommandMediator, HubCommandMediator>();

            services.AddSingleton<IHubMessageTypeMap, HubMessageTypeMap>();
        }
    }
}
