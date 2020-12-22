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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Postoffice
{
    public static class PeekMessages
    {
        private static readonly string _database = "energinetDocsDB";
        private static readonly string _container = "energinetDocsContainer";
        private static readonly string _urlDB = GetEnvironmentVariable("CosmosDbEndpoint");
        private static readonly string _key = GetEnvironmentVariable("CosmosDbKey");
        private static readonly string _actorIdColumnName = "RecipientMarketParticipant_mRID";

       [FunctionName("Peek")]
        public static async Task<IActionResult> PeekAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            log.LogInformation("C# HTTP trigger function processed a request.");
            //find a actorID in body or as a http parameter
            string actorID = req.Query["ActorID"];
            string responseMessage = string.Empty;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(requestBody) && string.IsNullOrEmpty(actorID))
            {
                dynamic data = JsonSerializer.Deserialize<PeekRequest>(requestBody, options);
                actorID = actorID ?? data?.ActorId;
            }

            if (string.IsNullOrWhiteSpace(actorID))
            {
                //no actorID provided in the body or http parameter. Return Bad Request
                return new BadRequestResult();
            }
            else
            {
                try
                {
                    //return previously peeked and unacknowledged Rejected messages if any
                    List<Measure> peekedRejectedMeasures = await CheckPrevRejectedAsync(actorID);
                    //if there are previously peeked rejected messages return them and send them
                    if (peekedRejectedMeasures.Count > 0) { return new OkObjectResult(peekedRejectedMeasures); }
                    //return previously peeked and unacknowledged Valid messages if any
                    List<Measure> peekedMeasures = await CheckPrevPeekedAsync(actorID);
                    //if there are peeked Valid messages return them and send them
                    if (peekedMeasures.Count > 0) { return new OkObjectResult(peekedMeasures); }

                    var newjobID = Guid.NewGuid().ToString();
                    //return rejected message,set it to peeked and send them
                    List<Measure> rejectedMeasures = await CheckNewRejectedAsync(actorID, newjobID, log);
                    if (rejectedMeasures.Count > 0) { return new OkObjectResult(rejectedMeasures); }
                    //return no peeked, set it to peeked and send them
                    List<Measure> peekMessages = await CheckNewPeekedAsync(actorID, newjobID, log);
                    if (peekMessages.Count > 0) { return new OkObjectResult(peekMessages); }
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                    return new BadRequestResult();
                }
             }

            return new OkObjectResult(string.Empty);
            //format
        }

        [FunctionName("Dequeue")]
        public static async Task<IActionResult> DequeueAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP Dequeue function processed a request.");
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //find a actorID and JobId in body or as a http parameter
            if (req == null)
            {
                return new BadRequestResult();
            }

            string actorID = req.Query["ActorID"];
            string jobID = req.Query["jobid"];
            string responseMessage = string.Empty;
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (!string.IsNullOrEmpty(requestBody) && (string.IsNullOrEmpty(jobID) || string.IsNullOrEmpty(actorID)))
            {
                dynamic data = JsonSerializer.Deserialize<DequeueRequest>(requestBody, options);
                actorID = actorID ?? data?.actorID;
                jobID = jobID ?? data?.jobID;
            }

            if (string.IsNullOrWhiteSpace(actorID) || string.IsNullOrWhiteSpace(jobID))
            {
                //either actorID or jobID were not provided in the body or http parameter. Return Bad Request
                return new BadRequestResult();
            }
            else
            {
                try
                {
                    //check if for given actorid there are unpeeked messages for the given jobid
                    bool validJobID = await IsJobIDValid(actorID, jobID);

                    if (validJobID)
                    {
                        //if so, Delete asynchronously
                        string query = $@"select * from c where c.{_actorIdColumnName} = @actorId and is_defined(c.PeekStatus) and c.JobID =@jobId";
                        var querydef = new QueryDefinition(query).WithParameter("@actorId", actorID).WithParameter("@jobId", jobID);

                        //Note: this should be async/not awaited: there should be a timer triggered Az function, that queries a queue. This function should just send the request to that queue
                        await BulkDelete(actorID, querydef, log);

                        // and send OK back
                        return new OkObjectResult("OK");
                    }
                    else
                    {
                        //else send NOK
                        return new BadRequestObjectResult($@"No previously unacknowledged request for jobid {jobID}");
                    }
                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                }
             }

            return new OkObjectResult(string.Empty);
            //format
        }

        private static async Task<bool> IsJobIDValid(string actorID, string jobID)
        {
            List<Measure> result;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                //check for previously peeked rejected messages with given jobID
                string query = $@"select top 1 * from c where c.{_actorIdColumnName} = @actorId and c.PeekStatus='Latest' and c.JobID = @jobId";
                var querydef = new QueryDefinition(query)
                                .WithParameter("@actorId", actorID)
                                .WithParameter("@jobId", jobID);
                //query measures with peek status true and actorID
                result = await QueryItemsAsync(querydef, client);
            }

            return result.Count > 0;
        }

        private static async Task<List<Measure>> CheckPrevRejectedAsync(string actorID)
        {
            List<Measure> result;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                //check for previously peeked rejected messages
                string query = $@"select * from c where c.MessageType='InvalidTimeSeries' and c.{_actorIdColumnName} = @actorId and c.PeekStatus='Latest'";
                var querydef = new QueryDefinition(query).WithParameter("@actorId", actorID);
                //query measures with peek status true and actorID
                result = await QueryItemsAsync(querydef, client);
            }

            return result;
        }

        private static async Task<List<Measure>> CheckPrevPeekedAsync(string actorID)
        {
            List<Measure> result;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                //check for previously peeked valid messages
                string query = $@"select * from c where c.MessageType='ValidObservation' and c.{_actorIdColumnName} = @actorId and c.PeekStatus='Latest'";
                var querydef = new QueryDefinition(query).WithParameter("@actorId", actorID);
                //query measures with peek status true and actorID
                result = await QueryItemsAsync(querydef, client);
            }

            return result;
        }

        private static async Task<List<Measure>> CheckNewRejectedAsync(string actorID, string jobID, ILogger log)
        {
            List<Measure> result;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                //call stored procedure to find where to set peeked status on new messages
                await BulkPeek("bulkPeekRejected", jobID, actorID, client, log);

                //Check if new rejected values are available
                string query = $@"select * from c where c.MessageType='InvalidTimeSeries' and is_defined(c.PeekStatus) and c.{_actorIdColumnName} = @actorId";
                var querydef = new QueryDefinition(query).WithParameter("@actorId", actorID);
                result = await QueryItemsAsync(querydef, client);
            }

            return result;
        }

        private static async Task<List<Measure>> CheckNewPeekedAsync(string actorID, string jobID, ILogger log)
        {
            List<Measure> result;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                //call stored procedure to find where to set peeked status on new messages
                await BulkPeek("bulkPeek", jobID, actorID, client, log);
                //extract values
                //var result = await QueryItemsAsync("select * from c where c.peeked = true and c.actorid=" + actorID, client);
                string query = $@"select * from c where c.MessageType='ValidObservation' and c.{_actorIdColumnName} = @actorId and c.PeekStatus='Latest'";
                var queryDef = new QueryDefinition(query).WithParameter("@actorId", actorID);
                result = await QueryItemsAsync(queryDef, client);
            }

            //partial delete: delete messages that are corrections
            string queryDelete = $@"select * from c where c.MessageType='ValidObservation' and c.{_actorIdColumnName} = @actorId and c.PeekStatus='Old'";
            var queryDefDelete = new QueryDefinition(queryDelete).WithParameter("@actorId", actorID);
            await BulkDelete(actorID, queryDefDelete, log);
            return result;
        }

        private static async Task BulkPeek(string sproc, string jobId, string actorid, CosmosClient client, ILogger log)
        {
            bool resume = true;
            do
            {
                StoredProcedureExecuteResponse<PeekStatus> result = await client.GetContainer(_database, _container).Scripts.ExecuteStoredProcedureAsync<PeekStatus>(sproc, new PartitionKey(actorid), new dynamic[] { jobId });

                log.LogInformation($"Batch Delete Completed.\tPeeked: {result.Resource.Peeked}\tContinue: {result.Resource.Continuation}");
                resume = result.Resource.Continuation;
            }
            while (resume);
        }

        private static async Task BulkDelete(string actorid, QueryDefinition query, ILogger log)
        {
            bool resume = true;
            using (CosmosClient client = new CosmosClient(_urlDB, _key))
            {
                do
                {
                    StoredProcedureExecuteResponse<DeleteStatus> result = await client.GetContainer(_database, _container).Scripts.ExecuteStoredProcedureAsync<DeleteStatus>("bulkDelete", new PartitionKey(actorid), new dynamic[] { query });

                    log.LogInformation($"Batch Delete Completed.\tDeleted: {result.Resource.Deleted}\tContinue: {result.Resource.Continuation}");
                    resume = result.Resource.Continuation;
                }
                while (resume);
            }
        }

        private static void SendResponse(string returnValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run a query (using Azure Cosmos DB SQL syntax) against the container
        /// </summary>
        private static async Task<List<Measure>> QueryItemsAsync(QueryDefinition query, CosmosClient client)
        {
            FeedIterator<Measure> queryResultSetIterator = client.GetContainer(_database, _container).GetItemQueryIterator<Measure>(query);

            List<Measure> measures = new List<Measure>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Measure> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (Measure mes in currentResultSet)
                {
                    measures.Add(mes);
                }
            }

            return measures;
        }

        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
