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
using System.Diagnostics;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protobuf
{
    public class Workload
    {
        public async Task<WorkloadResult> ExecuteAsync<TObject>(IObjectSerializer<TObject> serializer, TObject[] obj, MockFileSystem fileSystem)
        {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < obj.Length; i++)
            {
                await serializer.SerializeAsync(obj[i]);
            }

            var ts1 = sw.Elapsed;

            var totalBytes = fileSystem.AllFiles.Sum(f => fileSystem.FileInfo.FromFileName(f).Length);

            var streams = fileSystem.AllFiles.Select(f => fileSystem.File.OpenRead(f));

            sw.Restart();

            foreach (var stream in streams)
            {
                await serializer.DeserializeAsync(stream);
            }

            var ts2 = sw.Elapsed;
            sw.Stop();

            return new WorkloadResult(serializer.GetType().Name, ts1, ts2, totalBytes, obj.Length);
        }

        public Task<WorkloadResult> ExecuteAsync<TObject>(IObjectSerializer<TObject> serializer, TObject obj, MockFileSystem fileSystem)
        {
            return ExecuteAsync(serializer, new[] { obj }, fileSystem);
        }

        public class WorkloadResult
        {
            internal WorkloadResult(string name, TimeSpan serialization, TimeSpan deserialization, long totalBytes, int objectCount)
            {
                Name = name;
                Serialization = serialization;
                Deserialization = deserialization;
                TotalBytes = totalBytes;
                ObjectCount = objectCount;
            }

            public string Name { get; }

            public TimeSpan Serialization { get; }

            public TimeSpan Deserialization { get; }

            public long TotalBytes { get; }

            public int ObjectCount { get; }

            public long TotalKb => TotalBytes / 1024;

            public long TotalMb => TotalKb / 1024;

            public override string ToString()
            {
                var sb = new StringBuilder();

                sb.Append(nameof(Name));
                sb.Append(":\t");
                sb.Append(Name);
                sb.Append("\t");
                sb.Append(nameof(Serialization));
                sb.Append(":\t");
                sb.Append(Serialization);
                sb.Append("\t");
                sb.Append(nameof(Deserialization));
                sb.Append(":\t");
                sb.Append(Deserialization);
                sb.Append("\t");
                sb.Append(nameof(TotalBytes));
                sb.Append("\t");
                sb.Append(TotalBytes);
                sb.Append("\t");
                sb.Append(nameof(TotalMb));
                sb.Append("\t");
                sb.Append(TotalMb);
                sb.Append("\t");
                sb.Append(nameof(TotalKb));
                sb.Append("\t");
                sb.Append(TotalKb);

                return sb.ToString();
            }
        }
    }
}
