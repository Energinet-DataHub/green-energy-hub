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
using System.Linq;
using System.Threading.Tasks;
using Energinet.DataHub.SoapAdapter.Application.Converters;
using FluentAssertions;
using Xunit;

namespace Energinet.DataHub.SoapAdapter.Tests
{
    public class SendMessageResponseConverterTests
    {
        [Fact]
        public async Task Validate_converted_output()
        {
            await using (var input = File.OpenRead("Assets/ResponseJson.json"))
            {
                // Arrange
                var output = new MemoryStream();
                var messageReference = "MsgId-UV204-20120910-131249.0592";
                var sut = new SendMessageResponseConverter();

                // Act
                await sut.ConvertAsync(input, output, messageReference).ConfigureAwait(false);

                // This is annoying to test
                using (var reader = new StreamReader(output))
                {
                    var actual = await reader.ReadToEndAsync().ConfigureAwait(false);

                    // Assert
                    actual.Should().StartWith("<?xml version");
                    actual.Should().Contain(messageReference);
                }
            }
        }
    }
}
