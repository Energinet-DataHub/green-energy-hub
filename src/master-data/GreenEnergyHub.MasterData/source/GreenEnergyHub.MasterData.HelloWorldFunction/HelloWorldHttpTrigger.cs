using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GreenEnergyHub.MasterData.HelloWorldFunction
{
    public static class HelloWorldHttpTrigger
    {
        [FunctionName("HelloWorldHttpTrigger")]
        public static async Task<OkObjectResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            return await Task.FromResult(new OkObjectResult("Hello World"));
        }
    }
}
