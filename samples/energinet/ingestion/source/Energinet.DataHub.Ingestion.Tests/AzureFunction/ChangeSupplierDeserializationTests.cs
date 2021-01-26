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
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Application.ChangeOfSupplier;
using Energinet.DataHub.Ingestion.Infrastructure;
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging.MessageTypes.Common;
using GreenEnergyHub.TestHelpers.Traits;
using Microsoft.Extensions.Logging;
using NodaTime;
using NSubstitute;
using Xunit;

namespace Energinet.DataHub.Ingestion.Tests.AzureFunction
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class ChangeSupplierDeserializationTests
    {
        [Fact]
        public async Task Can_deserialize_incoming_json()
        {
            var logger = Substitute.For<ILogger<JsonMessageDeserializer>>();
            var serializer = new JsonMessageDeserializer(logger, new JsonSerializer());
            var targetType = typeof(ChangeOfSupplierMessage);

            var expected = new ChangeOfSupplierMessage();
            expected.BalanceResponsibleParty = new MarketParticipant("12345678", null, null, "VA");
            expected.EnergySupplier = new MarketParticipant("12345678", null, null, "VA");
            expected.Consumer = new MarketParticipant("1234567890", "Hans Hansen", null, "ARR");
            expected.MarketEvaluationPoint = new MarketEvaluationPoint("123456789123456789");
            expected.StartDate = Instant.FromUtc(2020, 9, 30, 22, 0, 0) + Duration.FromMilliseconds(1);
            expected.Transaction = new Transaction("a01dbf8b-ea99-4798-9bd4-ed85ecf79897");

            await using var fs = File.OpenRead("Assets/ChangeSupplier.json");

            var request = await serializer.RehydrateAsync(fs, targetType).ConfigureAwait(false);
            var actual = request as ChangeOfSupplierMessage;

            var compare = new ChangeSupplierEquality();
            Assert.Equal(expected, actual, compare);
        }

        private class ChangeSupplierEquality : IEqualityComparer<ChangeOfSupplierMessage?>
        {
            public bool Equals(ChangeOfSupplierMessage? x, ChangeOfSupplierMessage? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Transaction.Equals(y.Transaction) && x.BalanceResponsibleParty.Equals(y.BalanceResponsibleParty) && x.EnergySupplier.Equals(y.EnergySupplier) && x.Consumer.Equals(y.Consumer) && x.MarketEvaluationPoint.Equals(y.MarketEvaluationPoint) && x.StartDate.Equals(y.StartDate);
            }

            public int GetHashCode(ChangeOfSupplierMessage obj)
            {
                return HashCode.Combine(obj.Transaction, obj.BalanceResponsibleParty, obj.EnergySupplier, obj.Consumer, obj.MarketEvaluationPoint, obj.StartDate);
            }
        }
    }
}
