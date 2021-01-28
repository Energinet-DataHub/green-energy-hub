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

using System.IO;

namespace GreenEnergyHub.IntegrationTest.Helpers
{
    public static class StreamHelper
    {
        /// <summary>
        /// Used due to the way Parquet.NET accesses files we need to make a memory copy of the stream
        /// https://github.com/elastacloud/parquet-dotnet#reading-files
        /// </summary>
        /// <param name="stream">The stream you want to make a memory copy of</param>
        /// <returns>A new memory stream clone</returns>
        public static MemoryStream CopyToMemoryStream(Stream stream)
        {
            var memoryStream = new MemoryStream();
            using (stream)
            {
                stream?.CopyTo(memoryStream);
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
