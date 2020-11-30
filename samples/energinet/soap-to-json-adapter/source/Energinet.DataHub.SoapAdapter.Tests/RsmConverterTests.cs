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
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application.Converters;
using Xunit;

namespace Energinet.DataHub.SoapAdapter.Tests
{
    public class RsmConverterTests
    {
        [Fact]
        public async Task Validate_converted_output()
        {
            await using var ms = new MemoryStream();
            await using var fs = File.OpenRead("Assets/Rsm001CPR.xml");

            var rsm001 = new ChangeSupplierConverter();

            await rsm001.ConvertAsync(fs, ms).ConfigureAwait(false);

            ms.Position = 0;
            using var sr = new StreamReader(ms);
            var json = await sr.ReadToEndAsync().ConfigureAwait(false);

            Assert.True(json.Contains("SessId-0.58783300-1582196206", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("*****************", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("6311125", StringComparison.OrdinalIgnoreCase));
            Assert.True(json.Contains("11111111", StringComparison.OrdinalIgnoreCase));
        }
    }
}
