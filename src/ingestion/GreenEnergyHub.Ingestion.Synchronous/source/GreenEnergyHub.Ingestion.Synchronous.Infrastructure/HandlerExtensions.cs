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
using GreenEnergyHub.Ingestion.Synchronous.Application;
using GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestMediation;
using GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestRouting;
using GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RulesEngine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure
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
        /// to find the IHubActionHandlers, IHubActionRequests, IHubRuleSets,
        /// IEndpoints, IRuleEngines, the rules (represented as a list of
        /// Types), the IEndpointResolver, and the IHubRequestTypeMap, and to
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
            var assemblies = customerAssemblies.Append(typeof(IHubActionHandler<>).Assembly).ToArray();

            services.AddMediatR(assemblies);

            // Collection of IHubActionRequest types to discover related classes for
            var actionRequestsToRegister = new List<Type>();

            // Walk assemblies in order passed, discovering unique (by name) declared request types
            foreach (var assembly in assemblies)
            {
                var assemblyTypes = assembly.GetTypes();

                // Discover action requests but omit any already pending registration by same name from another assembly
                var actionRequests = assemblyTypes.Where(type => type.GetInterfaces().Contains(typeof(IHubActionRequest)));
                var newRequestTypes = actionRequests.Where(existingRequestType => actionRequestsToRegister.All(requestType => requestType.Name != existingRequestType.Name));
                actionRequestsToRegister.AddRange(newRequestTypes);
            }

            // Collection of IRuleSet types to discover related classes for
            var ruleSetsToRegister = new List<Type>();

            // With the set of unique action requests determined, discover rulesets based on assembly ordering
            foreach (var actionRequest in actionRequestsToRegister)
            {
                foreach (var assembly in assemblies)
                {
                    var assemblyTypes = assembly.GetTypes();

                    // Discover rule sets but omit any already pending registration by same name from another assembly
                    var ruleSet = assemblyTypes
                        .Where(type => type.GetInterfaces().Contains(typeof(IHubRuleSet<>).MakeGenericType(actionRequest)))
                        .Where(existingRuleSet => ruleSetsToRegister.All(ruleSetType => ruleSetType.Name != existingRuleSet.Name))
                        .FirstOrDefault();

                    if (ruleSet != default)
                    {
                        // Keep track of the registered rulesets to avoid duplicated registrations in next iterations
                        ruleSetsToRegister.Add(ruleSet);

                        // Register the discovered ruleset
                        services.AddScoped(typeof(IHubRuleSet<>).MakeGenericType(actionRequest), ruleSet);
                    }
                }
            }

            foreach (var actionRequest in actionRequestsToRegister)
            {
                // Register with mapping class for the request type
                services.AddTransient(_ => new RequestRegistration(actionRequest));

                // Register supporting classes
                // services.AddScoped(typeof(IEndpoint<>).MakeGenericType(actionRequest), endpointType.MakeGenericType(actionRequest));
                services.AddScoped(typeof(IRuleEngine<>).MakeGenericType(actionRequest), typeof(NRulesEngine<>).MakeGenericType(actionRequest));
            }

            services.AddSingleton<IHubRequestTypeMap, HubRequestTypeMap>();
            // services.AddScoped<IEndpointResolver, EndpointResolver>();
        }
    }
}
