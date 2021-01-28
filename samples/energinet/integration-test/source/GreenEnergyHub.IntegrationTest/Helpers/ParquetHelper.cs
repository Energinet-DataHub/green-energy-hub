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
using System.Linq;
using System.Threading.Tasks;
using GreenEnergyHub.IntegrationTest.Types;
using NodaTime;
using NodaTime.Calendars;
using Parquet;

namespace GreenEnergyHub.IntegrationTest.Helpers
{
    public class ParquetHelper
    {
        private readonly ParquetBlobStorageHelper _parquetBlobStorageHelper;

        public ParquetHelper(ParquetBlobStorageHelper parquetBlobStorageHelper)
        {
            _parquetBlobStorageHelper = parquetBlobStorageHelper;
        }

        /// <summary>
        /// This opens the partition schema using fileDate and then tries for x seconds to find the specified CorrelationID
        /// in a TimeSeries message and retries every second
        /// </summary>
        /// <param name="correlationId">Id of the time series document your searching for</param>
        /// <param name="maxWaitTimeInSeconds">How long time the method runs before throwing a timeout exception</param>
        /// <param name="fileDate">Used to navigate the partition schema of parquet as they are stored by date</param>
        /// <returns>All the time series found that matches CorrelationId in the date partition</returns>
        public async Task<List<ParquetTimeSeries>> FindTimeSeriesInBlobStorage(string correlationId, int maxWaitTimeInSeconds, LocalDate fileDate)
        {
            var waitUntil = SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromSeconds(maxWaitTimeInSeconds));
            do
            {
                var streams = await _parquetBlobStorageHelper
                    .DownloadParquetFilesStoredWithDatePartitionSchema(new LocalDate(Era.Common, fileDate.Year, fileDate.Month, fileDate.Day))
                    .ConfigureAwait(false);

                var allFoundTimeSeries = new List<ParquetTimeSeries>();
                foreach (var stream in streams)
                {
                    var memoryStream = StreamHelper.CopyToMemoryStream(stream);
                    var timeSeriesInStream = GetAllTimeSeriesFromStream(memoryStream);
                    allFoundTimeSeries.AddRange(timeSeriesInStream);
                }

                var timeSeries = allFoundTimeSeries.Where(x => x.CorrelationId == correlationId).ToList();

                if (timeSeries.Any())
                {
                    return timeSeries;
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
            while (waitUntil > SystemClock.Instance.GetCurrentInstant());

            throw new TimeoutException($"QueryItems timeout exceeded with {maxWaitTimeInSeconds} seconds");
        }

        public List<ParquetTimeSeries> GetAllTimeSeriesFromStream(Stream stream)
        {
            var result = new List<ParquetTimeSeries>();
            // open parquet file reader
            using var parquetReader = new ParquetReader(stream);
            // get file schema (available straight after opening parquet reader)
            // however, get only data fields as only they contain data values
            var dataFields = parquetReader.Schema.GetDataFields();

            // enumerate through row groups in this file
            for (var i = 0; i < parquetReader.RowGroupCount; i++)
            {
                // create row group reader
                using var groupReader = parquetReader.OpenRowGroupReader(i);
                // read all columns inside each row group (you have an option to read only
                // required columns if you need to.
                var columns = dataFields.Select(groupReader.ReadColumn).ToArray();

                // Here we count the number of rows with columns[0].Data.Length and then loop through
                // each row and reading each column
                for (var j = 0; j < columns[0].Data.Length; j++)
                {
                    var marketEvaluationPointMRID = (string)columns[0].Data.GetValue(j);
                    var time = (DateTimeOffset)columns[1].Data.GetValue(j);
                    var quantity = (decimal)columns[2].Data.GetValue(j);
                    var correlationId = (string)columns[3].Data.GetValue(j);
                    var messageReference = (string)columns[4].Data.GetValue(j);
                    var marketDocumentMrid = (string)columns[5].Data.GetValue(j);
                    var createdTime = (DateTimeOffset)columns[6].Data.GetValue(j);
                    var senderMarketParticipantMRID = (string)columns[7].Data.GetValue(j);
                    var processType = (string)columns[8].Data.GetValue(j);
                    var senderMarketParticipantMarketRoleType = (string)columns[9].Data.GetValue(j);
                    var timeSeriesMrid = (string)columns[10].Data.GetValue(j);
                    var mktActivityRecordsStatus = (string)columns[11].Data.GetValue(j);
                    var marketEvaluationPointType = (string)columns[12].Data.GetValue(j);
                    var quality = (string)columns[13].Data.GetValue(j);
                    var meterReadingPeriodicity = (string)columns[14].Data.GetValue(j);
                    var meteringMethod = (string)columns[15].Data.GetValue(j);
                    var meteringGridAreaDomainMRID = (string)columns[16].Data.GetValue(j);
                    var connectionState = (string)columns[17].Data.GetValue(j);
                    var energySupplierMarketParticipantMRID = (string)columns[18].Data.GetValue(j);
                    var balanceResponsiblePartyMarketParticipantMRID = (string)columns[19].Data.GetValue(j);
                    var inMeteringGridAreaDomainMRID = (string)columns[20].Data.GetValue(j);
                    var outMeteringGridAreaDomainMRID = (string)columns[21].Data.GetValue(j);
                    var parentDomainMRID = (string)columns[22].Data.GetValue(j);
                    var serviceCategoryKind = (string)columns[23].Data.GetValue(j);
                    var settlementMethod = (string)columns[24].Data.GetValue(j);
                    var quantityMeasurementUnitName = (string)columns[25].Data.GetValue(j);
                    var product = (string)columns[26].Data.GetValue(j);

                    var timeSeries = new ParquetTimeSeries
                    {
                        MarketEvaluationPointMRID = marketEvaluationPointMRID,
                        Time = Instant.FromDateTimeOffset(time),
                        Quantity = quantity,
                        CorrelationId = correlationId,
                        MessageReference = messageReference,
                        MarketDocumentMRID = marketDocumentMrid,
                        CreatedTime = Instant.FromDateTimeOffset(createdTime),
                        SenderMarketParticipantMRID = senderMarketParticipantMRID,
                        ProcessType = processType,
                        SenderMarketParticipantMarketRoleType = senderMarketParticipantMarketRoleType,
                        TimeSeriesMRID = timeSeriesMrid,
                        MktActivityRecordStatus = mktActivityRecordsStatus,
                        MarketEvaluationPointType = marketEvaluationPointType,
                        Quality = quality,
                        MeterReadingPeriodicity = meterReadingPeriodicity,
                        MeteringMethod = meteringMethod,
                        MeteringGridAreaDomainMRID = meteringGridAreaDomainMRID,
                        ConnectionState = connectionState,
                        EnergySupplierMarketParticipantMRID = energySupplierMarketParticipantMRID,
                        BalanceResponsiblePartyMarketParticipantMRID = balanceResponsiblePartyMarketParticipantMRID,
                        InMeteringGridAreaDomainMRID = inMeteringGridAreaDomainMRID,
                        OutMeteringGridAreaDomainMRID = outMeteringGridAreaDomainMRID,
                        ParentDomainMRID = parentDomainMRID,
                        ServiceCategoryKind = serviceCategoryKind,
                        SettlementMethod = settlementMethod,
                        QuantityMeasurementUnitName = quantityMeasurementUnitName,
                        Product = product,
                    };

                    result.Add(timeSeries);
                }
            }

            return result;
        }
    }
}
