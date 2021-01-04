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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Logging;
using NodaTime;
using TimeSeriesGenerator.Domain;

namespace TimeSeriesGenerator
{
    public class TimeSeriesGeneratorService : ITimeSeriesGeneratorService
    {
        private readonly int[] _gridAreas;
        private readonly ILogger<TimeSeriesGeneratorService> _logger;

        public TimeSeriesGeneratorService(ILogger<TimeSeriesGeneratorService> logger)
        {
            _logger = logger;
            _gridAreas = GridAreas.DanishAreas;
        }

        public TimeSeriesPoint[] GenerateTimeSeriesFromCsvFile(
            string csvFile,
            List<Instant> generatedTimeSpanSet,
            int numberOfMeteringPoints)
        {
            _logger.LogInformation("Generating data of csv file. Stand by....");

            var records = GetRecordsFromCsvFile(csvFile);
            records = records.Where(r =>
                r.PhysicalStatus == "E22" || r.PhysicalStatus == "E23"); //only add MPs that are functional

            var bag = new ConcurrentBag<TimeSeriesPoint>();
            Parallel.ForEach(records, record =>
            {
                var random = new Random();
                int.TryParse(record.Count, out var count);
                //the count contains the number of Metering points in this gridarea,MP type and SettlementType
                for (var i = 0; i < count; i++)
                {
                    var timeSeriesPoints = GenerateTimeSeriesPoints(
                        random,
                        generatedTimeSpanSet,
                        record.TypeOfMp,
                        record.SettlementMethod,
                        record.GridArea,
                        record.GridArea, //TODO this might need to be some sort of lookup to a neighbor area
                        numberOfMeteringPoints);

                    foreach (var tsp in timeSeriesPoints)
                    {
                        bag.Add(tsp);
                    }
                }
            });

            return bag.ToArray();
        }

        public List<Instant> GenerateTimeSpans(Instant startDate, Instant endTime, int resolution)
        {
            var list = new List<Instant>();
            var old = startDate;
            while (old < endTime)
            {
                old = old.Plus(Duration.FromMinutes(resolution));
                list.Add(old);
            }

            return list;
        }

        public TimeSeriesPoint[] ExchangeDataset(
            List<Instant> generatedTimeSpanSet,
            int numberOfMeteringPoints)
        {
            return GenerateDataset(50, "E20", "E02", generatedTimeSpanSet, numberOfMeteringPoints);
        }

        public TimeSeriesPoint[] ProductionDataset(
            List<Instant> generatedTimeSpanSet,
            int numberOfMeteringPoints)
        {
            return GenerateDataset(10, "E18", "E02", generatedTimeSpanSet, numberOfMeteringPoints);
        }

        public TimeSeriesPoint[] ConsumptionDataset(
            List<Instant> generatedTimeSpanSet,
            int numberOfGridAreas,
            int meteringPointsPerGridArea,
            int numberOfMeteringPoints)
        {
            var random = new Random();
            var sw = new Stopwatch();
            sw.Start();
            var result = new ConcurrentBag<TimeSeriesPoint>();
            Parallel.For(0, numberOfGridAreas, gridAreaIndex =>
            {
                var gridArea = _gridAreas[gridAreaIndex].ToString();
                for (var meteringPointInGridArea = 0;
                    meteringPointInGridArea < meteringPointsPerGridArea;
                    meteringPointInGridArea++)
                {
                    //Do a bit of progress status to the log
                    if (meteringPointInGridArea % 10000 == 0)
                    {
                        _logger.LogInformation($"Generated {meteringPointInGridArea} data for grid area {gridArea}");
                    }

                    var timeSeriesPackages = GenerateTimeSeriesPoints(
                        random,
                        generatedTimeSpanSet,
                        "E17",
                        string.Empty,
                        gridArea,
                        string.Empty,
                        numberOfMeteringPoints);

                    foreach (var tsp in timeSeriesPackages)
                    {
                        result.Add(tsp);
                    }
                }
            });
            sw.Stop();
            _logger.LogInformation("**************");
            _logger.LogInformation(
                $"Generated {result.Count} time series points in {sw.Elapsed.TotalSeconds:F1} seconds");
            _logger.LogInformation("**************");

            return result.ToArray();
        }

        private TimeSeriesPoint[] GenerateDataset(
            int parallelizeAcross,
            string marketEvaluationPointType,
            string settlementMethod,
            List<Instant> generatedTimeSpanSet,
            int numberOfMeteringPoints)
        {
            var random = new Random();
            var result = new ConcurrentBag<TimeSeriesPoint>();
            Parallel.For(0, parallelizeAcross, i =>
            {
                var timeSeriesPoints = GenerateTimeSeriesPoints(
                    random,
                    generatedTimeSpanSet,
                    marketEvaluationPointType,
                    settlementMethod,
                    _gridAreas[i].ToString(),
                    _gridAreas[i + 1].ToString(),
                    numberOfMeteringPoints);

                foreach (var tsp in timeSeriesPoints)
                {
                    result.Add(tsp);
                }
            });

            return result.ToArray();
        }

        private IEnumerable<SampleData> GetRecordsFromCsvFile(string csvFile)
        {
            IEnumerable<SampleData> records;
            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ";";
            records = csv.GetRecords<SampleData>().ToList();
            return records;
        }

        private IEnumerable<TimeSeriesPoint> GenerateTimeSeriesPoints(
            Random random,
            List<Instant> generatedTimeSpanSet,
            string marketEvaluationPointType,
            string settlementMethod,
            string gridAreaId,
            string exchangeGridAreaId,
            int numberOfMeteringPoints)
        {
            var tsp = new TimeSeriesPoint();

            TimeSeriesPointDummyGenerator.Massage(tsp, random, numberOfMeteringPoints);

            var list = new List<TimeSeriesPoint>();

            foreach (var dateTime in generatedTimeSpanSet)
            {
                tsp.ObservationTime = dateTime;
                tsp.MarketEvaluationPointType = marketEvaluationPointType; //E20,E17,E18
                if (string.IsNullOrWhiteSpace(settlementMethod))
                {
                    string[] settlementMethods = { "E02", "D01" };
                    tsp.SettlementMethod = settlementMethods[random.Next(settlementMethods.Length)];
                }
                else
                {
                    tsp.SettlementMethod = settlementMethod; // E02,D01
                }

                tsp.Quantity = random.NextDecimal(0, 100000);
                tsp.MeteringGridArea_Domain_mRID = gridAreaId;
                tsp.EnergySupplier_MarketParticipant_mRID = "8100000000030";
                tsp.BalanceResponsibleParty_MarketParticipant_mRID = "5790001330552";
                tsp.InMeteringGridArea_Domain_mRID = tsp.MeteringGridArea_Domain_mRID;
                //TODO ensure that we don't pull out the same ID
                tsp.OutMeteringGridArea_Domain_mRID = exchangeGridAreaId;
                list.Add(tsp);
            }

            return list;
        }
    }
}
