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
