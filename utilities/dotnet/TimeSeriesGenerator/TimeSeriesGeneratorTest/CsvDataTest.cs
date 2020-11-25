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
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using TimeSeriesGenerator;
using TimeSeriesGenerator.Domain;
using Xunit;

namespace TimeSeriesGeneratorTest
{
    public class CsvDataTest
    {
        private readonly ITimeSeriesGeneratorService _timeSeriesGenerator;
        private readonly int _generatedTimeSpanSetCount;
        private readonly IEnumerable<TimeSeriesPoint> _timeSeriesPoints;

        public CsvDataTest()
        {
            var logger = Mock.Of<ILogger<TimeSeriesGeneratorService>>();
            _timeSeriesGenerator = new TimeSeriesGeneratorService(logger);
            var start = new DateTime(2020, 10, 3, 0, 0, 0);
            var end = new DateTime(2020, 10, 3, 1, 0, 0);
            var generatedTimeSpanSet = _timeSeriesGenerator.GenerateTimeSpans(start, end, 60);
            _generatedTimeSpanSetCount = generatedTimeSpanSet.Count;
            _timeSeriesPoints =
                _timeSeriesGenerator.GenerateTimeSeriesFromCsvFile("DistributionOfMps.csv", generatedTimeSpanSet, 380000000);
        }

        [Fact]
        public void NumberOfTimeSeriesIs4183924()
        {
            Assert.Equal(4183924, _timeSeriesPoints.Count());
        }

        [Theory]
        [InlineData("791", "E17", "D01", 1016229)]
        [InlineData("791", "E17", "E02", 18696)]
        [InlineData("740", "E17", "D01", 396635)]
        [InlineData("740", "E17", "E02", 6769)]
        [InlineData("344", "E17", "D01", 319533)]
        [InlineData("344", "E17", "E02", 5933)]
        [InlineData("131", "E17", "D01", 396450)]
        [InlineData("131", "E17", "E02", 7770)]
        public void TestGridArea1(string gridId, string mPType, string sMethod, int expected)
        {
            var filter = _timeSeriesPoints.AsParallel().Where(t =>
                t.MeteringGridArea_Domain_mRID == gridId
                && t.MarketEvaluationPointType == mPType
                && t.SettlementMethod == sMethod);
            var count = filter.Count();
            Assert.Equal(_generatedTimeSpanSetCount * expected, count);
        }

        [Theory]
        [InlineData("791", "E18", 16051)]
        [InlineData("791", "E20", 206)]
        [InlineData("740", "E18", 15320)]
        [InlineData("740", "E20", 145)]
        [InlineData("344", "E18", 16319)]
        [InlineData("344", "E20", 208)]
        [InlineData("131", "E18", 20710)]
        [InlineData("131", "E20", 306)]
        public void TestGridArea2(string gridId, string mPType, int expected)
        {
            var filter = _timeSeriesPoints.AsParallel().Where(t =>
                t.MeteringGridArea_Domain_mRID == gridId
                && t.MarketEvaluationPointType == mPType);
            var count = filter.Count();
            Assert.Equal(_generatedTimeSpanSetCount * expected, count);
        }

        [Fact]
        public void TestQuantityIsNotZero()
        {
            var filter = _timeSeriesPoints.Take(10000).Where(s => s.Quantity == 0);
            var count = filter.Count();
            Assert.Equal(0, count);
        }
    }
}
