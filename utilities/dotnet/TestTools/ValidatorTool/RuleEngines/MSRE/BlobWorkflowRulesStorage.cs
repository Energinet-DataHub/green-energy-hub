using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RulesEngine.Models;

namespace ValidatorTool.RuleEngines.MSRE {
    /// <summary>
    /// Support class to facilitate reading MSRE WorkflowRules from blob storage
    /// </summary>
    public class BlobWorkflowRulesStorage : IWorkflowRulesStorage {

        private BlobClient blobClient;
        private static List<WorkflowRules> _rules = null;

        public BlobWorkflowRulesStorage(string connectionString, string containerName, string blobName) {
            // Create necessary clients
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            blobClient = containerClient.GetBlobClient(blobName);
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
                using (MemoryStream s = new MemoryStream())
                {
                    await blobClient.DownloadToAsync(s);
                    s.Seek(0, SeekOrigin.Begin);
                    using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            _rules = serializer.Deserialize<List<WorkflowRules>>(reader);
                            return _rules;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw(ex);
            }
        }
    }
}
