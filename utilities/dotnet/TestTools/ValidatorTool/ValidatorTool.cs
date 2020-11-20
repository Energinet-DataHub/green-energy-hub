using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ValidatorTool.RuleEngines;

namespace ValidatorTool
{
    /// <summary>
    /// Read from the input queue, and validate incoming messages. Write results
    /// to the output queue.
    ///
    /// Use the console app with argument 'enqueue' to produce new messages on
    /// the input queue, and with argument 'output' to monitor the results of
    /// this function.
    /// </summary>
    public class Validator
    {
        private readonly IRuleEngine _ruleEngine;

        public Validator(IRuleEngine ruleEngine)
        {
            _ruleEngine = ruleEngine;
        }

        [FunctionName("ValidatorTool")]
        public async Task Run(
            [EventHubTrigger("%InputEventHubName%", Connection = "InputConnectionString")] EventData[] events,
            [EventHub("%OutputEventHubName%", Connection = "OutputConnectionString")] IAsyncCollector<string> outputEvents,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (var eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    // send to rule validator
                    var message = JsonSerializer.Deserialize<MeterMessage>(messageBody);
                    var result = await _ruleEngine.ValidateAsync(message);
                    var outputObject = $"{{\"validationResult\": {result}, \"message\": \"{messageBody}\"}}";
                    log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    // Send to other event hub
                    await outputEvents.AddAsync(outputObject);
                    await Task.Yield();
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }

            if (exceptions.Count == 1)
            {
                throw exceptions.Single();
            }
        }
    }
}
