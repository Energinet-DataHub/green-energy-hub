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
using Json.Schema;
using Microsoft.Extensions.DependencyInjection;

namespace GreenEnergyHub.Schemas.Json.Integration.ServiceCollection
{
    public static class ServiceCollectionExtension
    {
        public static void AddJsonValidation(this IServiceCollection services)
        {
            services.AddJsonValidation(options =>
            {
                options.OutputFormat = OutputFormat.Basic;
                options.RequireFormatValidation = true;
                options.ValidateAs = Draft.Draft7;
            });
        }

        public static void AddJsonValidation<TSchemaValidation>(
            this IServiceCollection services,
            Action<ValidationOptions> configure)
        where TSchemaValidation : class, IJsonSchemaProvider
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new ValidationOptions();
            configure(options);

            services.AddTransient(_ => options);
            services.AddSingleton<GreenEnergyHubSchemaValidator>();
            services.AddSingleton<IJsonSchemaProvider, TSchemaValidation>();
        }

        public static void AddJsonValidation(this IServiceCollection services, Action<ValidationOptions> configure)
        {
            services.AddJsonValidation<GreenEnergyHubJsonSchemaProvider>(configure);
        }
    }
}
