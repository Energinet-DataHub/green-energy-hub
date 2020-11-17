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
using System.Text.Json;
using System.Threading.Tasks;

namespace Protobuf.Implementations.Json
{
    public class JsonTimeSeriesSerializer :
        IObjectSerializer<Timeseries>,
        IObjectSerializer<Timeseries[]>
    {
        private readonly IFileSystem _fileSystem;

        public JsonTimeSeriesSerializer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Task SerializeAsync(Timeseries obj)
        {
            return SaveToFileSystemAsync($"{Guid.NewGuid():N}.json", obj);
        }

        async Task<Timeseries> IObjectSerializer<Timeseries>.DeserializeAsync(Stream data)
        {
            return await JsonSerializer.DeserializeAsync<Timeseries>(data);
        }

        public Task SerializeAsync(Timeseries[] obj)
        {
            return SaveToFileSystemAsync($"{Guid.NewGuid():N}.json", obj);
        }

        async Task<Timeseries[]> IObjectSerializer<Timeseries[]>.DeserializeAsync(Stream data)
        {
            return await JsonSerializer.DeserializeAsync<Timeseries[]>(data);
        }

        private async Task SaveToFileSystemAsync<TObject>(string filename, TObject obj)
        {
            await using var fs = _fileSystem.File.Create(filename);
            await JsonSerializer.SerializeAsync(fs, obj);
            await fs.FlushAsync();
        }
    }
}