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
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using GreenEnergyHub.IntegrationTest.Settings;
using NodaTime;

namespace GreenEnergyHub.IntegrationTest.Helpers
{
    public class ParquetBlobStorageHelper
    {
        private readonly BlobContainerClient _blobContainerClient;

        public ParquetBlobStorageHelper(BlobStorageSettings blobStorageSettings)
        {
            _blobContainerClient = new BlobContainerClient(blobStorageSettings.ConnectionString, blobStorageSettings.ContainerName);
        }

        public async Task<List<Stream>> DownloadParquetFilesStoredWithDatePartitionSchema(LocalDate partitionDateTime)
        {
            var files = new List<Stream>();
            var dict = new Dictionary<string, DateTimeOffset>();

            await foreach (var blob in _blobContainerClient.GetBlobsAsync())
            {
                var blobClient = _blobContainerClient.GetBlobClient(blob.Name);
                if (!blob.Name.Contains("checkpoint") &&
                    blob.Name.EndsWith("parquet", StringComparison.InvariantCultureIgnoreCase) &&
                    blob.Name.Contains($"year={partitionDateTime.Year}/month={partitionDateTime.Month}/day={partitionDateTime.Day}"))
                {
                    var downloadedBlob = await blobClient.DownloadAsync().ConfigureAwait(false);
                    files.Add(downloadedBlob.Value.Content);
                    dict.Add(blob.Name, downloadedBlob.Value.Details.LastModified);
                }
            }

            return files;
        }
    }
}
