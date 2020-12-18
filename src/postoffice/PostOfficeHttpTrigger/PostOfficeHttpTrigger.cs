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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Postoffice
{
    public static class PostOfficeHttpTrigger
    {
        [FunctionName("httpTrigger")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "put", "post", "delete", Route = null)] HttpRequest httpRequest,
            ILogger log)
        {
            if (httpRequest != null)
            {
                log.LogInformation("Post Office HTTP endpoint processed a request.");
                string id = httpRequest.Query["id"];
                /*
                //Uncomment to create new DB and Collection
                _ = await CosmosDbProvider.CreateAsync(
                    log,
                    GetEnvironmentVariable("CosmosDbEndpoint"),
                    GetEnvironmentVariable("CosmosDbKey")).ConfigureAwait(false);
                */

                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ArgumentException($"Parameter \"id\" should be specified.");
                }

                switch (httpRequest.Method)
                {
                    case "PUT":
                        return await AddDocAsync(httpRequest, id, log).ConfigureAwait(false);
                    case "POST":
                        return await AddDocAsync(httpRequest, id, log).ConfigureAwait(false);
                    case "GET":
                        return await GetDocAsync(id, log).ConfigureAwait(false);
                    case "DELETE":
                        return await DeleteDocAsync(id, log).ConfigureAwait(false);
                    default:
                        throw new NotImplementedException($"Method \"{httpRequest.Method}\" is not implemented in this function.");
                }
            }

            throw new NullReferenceException();
        }

        private static async Task<IActionResult> AddDocAsync(HttpRequest req, string id, ILogger log)
        {
            using (var streamReader = new StreamReader(req.Body))
            {
                string requestBody = await streamReader.ReadToEndAsync().ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(requestBody))
                {
                    using (var provider = new CosmosDbProvider(
                        log,
                        GetEnvironmentVariable("CosmosDbEndpoint"),
                        GetEnvironmentVariable("CosmosDbKey")))
                    {
                        await provider.AddDocAsync(id, requestBody).ConfigureAwait(false);
                    }

                    return new OkObjectResult($"Document {id} has been added successfully.");
                }
                else
                {
                    throw new InvalidDataException($"Body should not be empty for \"{req.Method}\" method");
                }
            }
        }

        private static async Task<IActionResult> GetDocAsync(string id, ILogger log)
        {
            using (var provider = new CosmosDbProvider(
                log,
                GetEnvironmentVariable("CosmosDbEndpoint"),
                GetEnvironmentVariable("CosmosDbKey")))
            {
                var doc = await provider.GetDocAsync(id).ConfigureAwait(false);
                return new OkObjectResult(doc.ToString());
            }
        }

        private static async Task<IActionResult> DeleteDocAsync(string id, ILogger log)
        {
            using (var provider = new CosmosDbProvider(
                log,
                GetEnvironmentVariable("CosmosDbEndpoint"),
                GetEnvironmentVariable("CosmosDbKey")))
            {
                await provider.DeleteDocAsync(id).ConfigureAwait(false);
            }

            return new OkObjectResult($"Document {id} has been deleted successfully.");
        }

        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
