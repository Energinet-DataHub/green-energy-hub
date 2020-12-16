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
using System.Text.Json;
using System.Threading.Tasks;
using GreenEnergyHub.Schemas.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.JsonValidation.EntryPoint
{
    public class SchemaValidator
    {
        private readonly GreenEnergyHubSchemaValidator _schemaValidator;

        public SchemaValidator(GreenEnergyHubSchemaValidator schemaValidator)
        {
            _schemaValidator = schemaValidator;
        }

        [FunctionName(nameof(SchemaValidator))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{type:alpha}")] [NotNull]
            HttpRequest req,
            ILogger log,
            string type)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            if (!SchemaType.TryParse(type, out var schemaType))
            {
                return new NotFoundObjectResult("Schema not found");
            }

            log.LogInformation($"Validating request against schema {schemaType}.", schemaType.Name);

            var json = await JsonDocument.ParseAsync(req.Body).ConfigureAwait(false);

            if (_schemaValidator.ValidateDocument(schemaType, json))
            {
                return new OkObjectResult("Schema valid");
            }

            return new BadRequestObjectResult("Schema invalid");
        }
    }
}
