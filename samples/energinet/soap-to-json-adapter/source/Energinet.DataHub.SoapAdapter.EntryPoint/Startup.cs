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
using Energinet.DataHub.SoapAdapter.Application;
using Energinet.DataHub.SoapAdapter.Application.Converters;
using Energinet.DataHub.SoapAdapter.Application.Infrastructure;
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.EntryPoint;
using Energinet.DataHub.SoapAdapter.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Energinet.DataHub.SoapAdapter.EntryPoint
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            // Register services
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ISendMessageService, SendMessageService>();
            builder.Services.AddScoped<RsmValidationParser>();
            builder.Services.AddScoped<IIngestionClient, IngestionClient>();
            builder.Services.AddSingleton<IRequestConverterFactory, RequestConverterFactory>();
            builder.Services.AddSingleton<IResponseConverterFactory, ResponseConverterFactory>();
            builder.Services.AddSingleton<IErrorResponseFactory, ErrorResponseFactory>();

            builder.Services.AddSingleton(
                serviceProvider =>
                {
                    var configuration = serviceProvider.GetService<IConfiguration>();
                    const string synchronousIngestorBaseUrl = "SYNCHRONOUS_INGESTOR_BASE_URL";
                    var endpoint = configuration.GetValue<string>(synchronousIngestorBaseUrl);
                    if (string.IsNullOrWhiteSpace(endpoint))
                    {
                        throw new ArgumentException($"'{synchronousIngestorBaseUrl}' cannot be null or empty");
                    }

                    return new IngestionClientSettings(endpoint);
                });
        }
    }
}
