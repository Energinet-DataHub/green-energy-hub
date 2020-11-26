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
using System.IO;
using GreenEnergyHub.JSONSchemaValidator.Validate;
using JetBrains.Annotations;
using Json.Schema;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace GreenEnergyHub.JSONSchemaValidator.Validate
{
    public class Startup : FunctionsStartup
    {
        public override void Configure([NotNull] IFunctionsHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Register services
            SchemaRegistry.Global.Register(new Uri("https://github.com/green-energy-hub/schemas"), SchemaHelper.CimDefinitions);
            builder.Services.AddTransient(_ => new ValidationOptions { OutputFormat = OutputFormat.Basic, ValidateFormat = true, ValidateAs = Draft.Draft7 });
            builder.Services.AddSingleton<ValidateService>();
        }
    }
}
