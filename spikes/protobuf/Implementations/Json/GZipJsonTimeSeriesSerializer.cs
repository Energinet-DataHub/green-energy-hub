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
using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Protobuf.Implementations.Json
{
    public class GZipJsonTimeSeriesSerializer :
        IObjectSerializer<Timeseries>,
        IObjectSerializer<Timeseries[]>
    {
        private readonly IFileSystem _fileSystem;

        public GZipJsonTimeSeriesSerializer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task SerializeAsync(Timeseries obj)
        {
            await using var fs = _fileSystem.File.Create(NewFilename());
            await using var zip = new GZipStream(fs, CompressionMode.Compress);
            await JsonSerializer.SerializeAsync(zip, obj);
        }

        async Task<Timeseries> IObjectSerializer<Timeseries>.DeserializeAsync(Stream data)
        {
            await using var zip = new GZipStream(data, CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<Timeseries>(zip);
        }

        public async Task SerializeAsync(Timeseries[] obj)
        {
            await using var fs = _fileSystem.File.Create(NewFilename());
            await using var zip = new GZipStream(fs, CompressionMode.Compress);
            await JsonSerializer.SerializeAsync(zip, obj);
        }

        async Task<Timeseries[]> IObjectSerializer<Timeseries[]>.DeserializeAsync(Stream data)
        {
            await using var zip = new GZipStream(data, CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<Timeseries[]>(zip);
        }

        private static string NewFilename()
        {
            return $"{Guid.NewGuid():N}.json";
        }
    }
}
