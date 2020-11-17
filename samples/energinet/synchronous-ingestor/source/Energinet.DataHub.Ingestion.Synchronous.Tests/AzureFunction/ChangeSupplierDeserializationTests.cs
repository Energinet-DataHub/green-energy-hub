using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Energinet.DataHub.Ingestion.Synchronous.Application.Requests;
using Energinet.DataHub.Ingestion.Synchronous.AzureFunction;
using Energinet.DataHub.Ingestion.Synchronous.Infrastructure;
using GreenEnergyHub.Ingestion.RequestTypes.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Energinet.DataHub.Ingestion.Synchronous.Tests.AzureFunction
{
    public class ChangeSupplierDeserializationTests
    {
        [Fact]
        public async Task Can_deserialize_incoming_json()
        {
            var logger = Substitute.For<ILogger<JsonMessageDeserializer>>();
            var serializer = new JsonMessageDeserializer(logger);
            var targetType = typeof(ChangeOfSupplierRequest);

            var expected = new ChangeOfSupplierRequest();
            expected.BalanceResponsibleParty.MRid = new MRid("12345678", "VA");
            expected.EnergySupplier.MRid = new MRid("12345678", "VA");
            expected.Consumer.Name = "Hans Hansen";
            expected.Consumer.MRid = new MRid("1234567890", "ARR");
            expected.MarketEvaluationPoint = new MarketEvaluationPoint("123456789123456789");
            expected.StartDate = new DateTime(2020, 9, 30, 22, 0, 0, 1, DateTimeKind.Utc);
            expected.Transaction = new Transaction("a01dbf8b-ea99-4798-9bd4-ed85ecf79897");

            await using var fs = File.OpenRead("Assets/ChangeSupplier.json");

            var request = await serializer.RehydrateAsync(fs, targetType).ConfigureAwait(false);
            var actual = request as ChangeOfSupplierRequest;

            var compare = new ChangeSupplierEquality();
            Assert.Equal(expected, actual, compare);
        }

        private class ChangeSupplierEquality : IEqualityComparer<ChangeOfSupplierRequest?>
        {
            public bool Equals(ChangeOfSupplierRequest? x, ChangeOfSupplierRequest? y)
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

            public int GetHashCode(ChangeOfSupplierRequest obj)
            {
                return HashCode.Combine(obj.Transaction, obj.BalanceResponsibleParty, obj.EnergySupplier, obj.Consumer, obj.MarketEvaluationPoint, obj.StartDate);
            }
        }
    }
}
