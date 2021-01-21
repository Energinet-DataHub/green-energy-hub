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
using System.Linq;
using System.Reflection;
using GreenEnergyHub.Messaging.Validation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Messaging.Integration.ServiceCollection
{
    internal static class ServiceCollectionValidationExtension
    {
        private static readonly Type _ruleSetDefinitionType = typeof(RuleCollection<>);
        private static readonly Type _propertyRuleType = typeof(PropertyRule<>);

        internal static IServiceCollection DiscoverValidation(this IServiceCollection serviceCollection, Assembly[] targetAssemblies)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (targetAssemblies == null) throw new ArgumentNullException(nameof(targetAssemblies));

            var allTypes = targetAssemblies.SelectMany(a => a.GetTypes());

            foreach (var type in allTypes)
            {
                // Check if the type is a ruleset and add it
                if (TryGetRuleSetDefinition(type, out var ruleSetServiceDescriptor))
                {
                    serviceCollection.Add(ruleSetServiceDescriptor);
                }

                // Check if the type is a ruleset and configure the rule engine to support it
                if (TryGetRuleEngineServiceDescriptor(type, out var ruleEngineServiceDescriptor))
                {
                    serviceCollection.Add(ruleEngineServiceDescriptor);
                }

                // Check if the type is a property rule and add it
                if (TryGetPropertyRuleServiceDescriptor(type, out var propertyRuleServiceDescriptor))
                {
                    serviceCollection.Add(propertyRuleServiceDescriptor);
                }
            }

            // Add our delegate as a singleton
            serviceCollection.AddSingleton<ServiceProviderDelegate>(sp => sp.GetService);

            return serviceCollection;
        }

        private static bool TryGetRuleSetDefinition(Type type, out ServiceDescriptor? serviceDescriptor)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            serviceDescriptor = GetServiceDescriptor(type, _ruleSetDefinitionType, CreateSingletonGeneric);

            return serviceDescriptor != null;
        }

        private static bool TryGetPropertyRuleServiceDescriptor(Type type, out ServiceDescriptor? serviceDescriptor)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            serviceDescriptor = GetServiceDescriptor(type, _propertyRuleType, CreateSingletonImplementationType);

            return serviceDescriptor != null;
        }

        private static bool TryGetRuleEngineServiceDescriptor(Type type, out ServiceDescriptor? serviceDescriptor)
        {
            static ServiceDescriptor Creator(Type baseType, Type messageType, Type implementationType) =>
                ServiceDescriptor.Singleton(typeof(IRuleEngine<>).MakeGenericType(messageType), typeof(FluentHybridRuleEngine<>).MakeGenericType(messageType));

            if (type == null) throw new ArgumentNullException(nameof(type));
            serviceDescriptor = GetServiceDescriptor(type, _ruleSetDefinitionType, Creator);

            return serviceDescriptor != null;
        }

        private static ServiceDescriptor? GetServiceDescriptor(Type type, Type baseType, Func<Type, Type, Type, ServiceDescriptor> creator)
        {
            var baseTypeIsGenericType = type.BaseType?.IsGenericType ?? false;
            if (!baseTypeIsGenericType) return null;

            var isRuleSetDefinition = type.BaseType?.GetGenericTypeDefinition().IsAssignableFrom(baseType) ?? false;
            if (!isRuleSetDefinition) return null;

            var genericType = type.BaseType?.GetGenericArguments().FirstOrDefault();
            if (genericType == null) return null;

            return creator(baseType, genericType, type);
        }

        private static ServiceDescriptor CreateSingletonGeneric(Type baseType, Type genericType, Type implementationType)
        {
            var serviceType = baseType.MakeGenericType(genericType);
            return ServiceDescriptor.Singleton(serviceType, implementationType);
        }

        private static ServiceDescriptor CreateSingletonImplementationType(Type baseType, Type genericType, Type implementationType)
        {
            return ServiceDescriptor.Singleton(implementationType, implementationType);
        }
    }
}
