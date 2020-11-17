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
using System.Threading.Tasks;
using Google.Protobuf;
using SpikeProtobuf.Implementations.Protobuf;

namespace Protobuf.Implementations.Protobuf
{
    public class ProtobufTimeSeriesSerializer :
        IObjectSerializer<Timeseries>,
        IObjectSerializer<TimeseriesCollection>
    {
        private readonly IFileSystem _fileSystem;

        public ProtobufTimeSeriesSerializer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Task SerializeAsync(Timeseries obj)
        {
            using var fs = _fileSystem.File.Create($"{Guid.NewGuid():N}.dat");
            obj.WriteTo(fs);

            return Task.CompletedTask;
        }

        Task<Timeseries> IObjectSerializer<Timeseries>.DeserializeAsync(Stream data)
        {
            var obj = Timeseries.Parser.ParseFrom(data);
            return Task.FromResult(obj);
        }

        public Task SerializeAsync(TimeseriesCollection obj)
        {
            using var fs = _fileSystem.File.Create($"{Guid.NewGuid():N}.dat");
            obj.WriteTo(fs);

            return Task.CompletedTask;
        }

        Task<TimeseriesCollection> IObjectSerializer<TimeseriesCollection>.DeserializeAsync(Stream data)
        {
            var obj = TimeseriesCollection.Parser.ParseFrom(data);
            return Task.FromResult(obj);
        }
    }
}
