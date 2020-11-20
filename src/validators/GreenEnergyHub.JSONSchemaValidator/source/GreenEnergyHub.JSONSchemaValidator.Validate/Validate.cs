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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.JSONSchemaValidator.Validate
{
    /// <summary>
    /// Class used for validation
    /// </summary>
    public class Validate
    {
        private readonly ValidateService _validateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Validate"/> class.
        /// </summary>
        /// <param name="validateService">The service that handles the actual validation</param>
        public Validate(ValidateService validateService)
        {
            _validateService = validateService;
        }

        /// <summary>
        /// Run the validation
        /// </summary>
        /// <param name="req">HTTP Request</param>
        /// <param name="log">Log Instance</param>
        /// <param name="type">Schema category</param>
        /// <returns>Validation output</returns>
        [FunctionName("SchemaValidator")]
        public async Task<IActionResult> RunAsync (
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{type:alpha}")] HttpRequest req,
            ILogger log,
            string type)
        {
            if (!Enum.TryParse<SchemaType>(type, out var schemaType))
            {
                return new NotFoundObjectResult("Schema not found");
            }

            log.LogInformation($"Validating request against schema {schemaType}.", type);

            var json = await JsonDocument.ParseAsync(req.Body);

            var validationResult = _validateService.ValidateDocument(schemaType, json);

            if (!validationResult.IsValid)
            {
                return new BadRequestObjectResult("Schema invalid");
            }

            return new OkObjectResult("Schema valid");
        }
    }
}
