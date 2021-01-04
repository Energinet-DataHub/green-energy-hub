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
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.Domain.Validation;
using NodaTime;
using Xunit;

namespace Energinet.DataHub.SoapAdapter.Tests
{
    public class RsmValidationParserTests
    {
        [Fact]
        public async Task Parser_should_fill_header()
        {
            await using var fs = File.OpenRead("Assets/Rsm001CPR.xml");

            var sut = new RsmValidationParser();
            Context actual = await sut.ParseAsync(fs).ConfigureAwait(false);

            Assert.NotNull(actual);

            var expected = new RsmHeader
            {
                Creation = Instant.FromUtc(2020, 02, 20, 10, 56, 46),
                Identification = "MsgId-0.58755000-1582196206",
                DocumentType = "392",
                RecipientIdentification = "5790001330552",
                SenderIdentification = "5790002263057",
                EnergyBusinessProcess = "E03",
                EnergyIndustryClassification = "23",
                EnergyBusinessProcessRole = "DDQ",
                MessageReference = null,
            };

            Assert.Equal(expected, actual.RsmHeader);
        }
    }
}
