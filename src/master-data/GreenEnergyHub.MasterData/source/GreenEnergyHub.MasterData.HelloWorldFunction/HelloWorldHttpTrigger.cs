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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.MasterData.HelloWorldFunction
{
    /// <summary>
    /// Sample Azure Function class
    /// </summary>
    public static class HelloWorldHttpTrigger
    {
        /// <summary>
        /// Sample Azure Function run method
        /// </summary>
        /// <param name="req">The HTTPRewquest received.</param>
        /// <param name="log">A logger instance.</param>
        /// <returns>A 200 OK result.</returns>
        [FunctionName("HelloWorldHttpTrigger")]
        public static async Task<OkObjectResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            var foo = req.Body; // do something with request here

            log.LogInformation("C# HTTP trigger function processed a request.");
            return await Task.FromResult(new OkObjectResult("Hello World")).ConfigureAwait(false);
        }
    }
}
