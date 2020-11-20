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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.SoapValidation.EntryPoint
{
    public class ValidateFunction
    {
        private const string FaultString = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"><soapenv:Body><soapenv:Fault><faultcode>soapenv:Client</faultcode><faultstring>{0}</faultstring><faultactor /></soapenv:Fault></soapenv:Body></soapenv:Envelope";

        private readonly IXmlSchemaValidator _xmlSchemaValidator;

        public ValidateFunction(
            IXmlSchemaValidator xmlSchemaValidator)
        {
            _xmlSchemaValidator = xmlSchemaValidator;
        }

        [FunctionName("ValidateFunction")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest httpRequest,
            ILogger logger)
        {
            logger.LogInformation("Adapting message");

            var requestId = httpRequest.Headers["RequestId"].SingleOrDefault();
            var result = await _xmlSchemaValidator.ValidateStreamAsync(httpRequest.Body);

            if (result.IsSuccess)
            {
                return new OkResult();
            }
            else
            {
                return new ContentResult
                {
                    Content = string.Format(FaultString, "B2B-005:" + requestId),
                    ContentType = "application/xml",
                    StatusCode = StatusCodes.Status400BadRequest,
                };
            }
        }
    }
}
