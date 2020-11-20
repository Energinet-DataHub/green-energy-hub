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
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using RulesEngine.Models;

namespace ValidatorTool.RuleEngines.MSRE
{
    /// <summary>
    /// Support class to facilitate reading MSRE WorkflowRules from blob storage
    /// </summary>
    public class BlobWorkflowRulesStorage : IWorkflowRulesStorage
    {
        private static List<WorkflowRules> _rules = null;

        private readonly BlobClient _blobClient;

        public BlobWorkflowRulesStorage(string connectionString, string containerName, string blobName)
        {
            // Create necessary clients
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            _blobClient = containerClient.GetBlobClient(blobName);
        }

        public async Task<List<WorkflowRules>> GetRulesAsync()
        {
            if (_rules != null)
            {
                // Return cached results if it is available
                return _rules;
            }

            try
            {
                using (var s = new MemoryStream())
                {
                    await _blobClient.DownloadToAsync(s);
                    s.Seek(0, SeekOrigin.Begin);
                    using (var sr = new StreamReader(s, Encoding.UTF8))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            var serializer = new JsonSerializer();
                            _rules = serializer.Deserialize<List<WorkflowRules>>(reader);
                            return _rules;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
