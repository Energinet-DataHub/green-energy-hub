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

namespace TimeSeriesGenerator.Domain
{
    /// <summary>
    ///     This class abstracts the expected time series data including joined master data
    ///     Naming convention is based on CIM
    /// </summary>
    public class TimeSeriesPoint
    {
      public string CorrelationId { get; set; }

        public string MessageReference { get; set; }

        public string MarketDocument_mRID { get; set; }

        /// <summary>
        ///     UTC time
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        public string SenderMarketParticipant_mRID { get; set; }

        public string ProcessType { get; set; }

        public string SenderMarketParticipantMarketRole_Type { get; set; }

        public string TimeSeries_mRID { get; set; }

        public string MktActivityRecord_Status { get; set; }

        public string Product { get; set; }

        public string QuantityMeasurementUnit_Name { get; set; }

        /// <summary>
        ///     Type of metering point E20,E17,E18
        /// </summary>
        public string MarketEvaluationPointType { get; set; }

        /// <summary>
        ///     Settlement method E02, D01
        /// </summary>
        public string SettlementMethod { get; set; }

        public string MarketEvaluationPoint_mRID { get; set; }

        /// <summary>
        ///     E17 (hour)
        ///     < 100.000
        ///         E17( flex) < 1.000.000
        ///                        E18( production) < 1.000.000
        ///                                             E20( exchanging) < 1.000.000
        /// </summary>
        public decimal Quantity { get; set; }

        public string Quality { get; set; }

        public DateTime ObservationTime { get; set; }

        /*Items below are joined master data*/

        public string MeteringMethod { get; set; }

        public string MeterReadingPeriodicity { get; set; }

        /// <summary>
        ///     Grid area (CSV FIL)
        /// </summary>
        public string MeteringGridArea_Domain_mRID { get; set; }

        public string ConnectionState { get; set; }

        /// <summary>
        ///     Balance Supplier 8100000000030
        /// </summary>
        public string EnergySupplier_MarketParticipant_mRID { get; set; }

        /// <summary>
        ///     Balance responsible 5790001330552
        /// </summary>
        public string BalanceResponsibleParty_MarketParticipant_mRID { get; set; }

        /// <summary>
        ///     Neighbor grid area (CSV fil)
        /// </summary>
        public string InMeteringGridArea_Domain_mRID { get; set; }

        /// <summary>
        ///     Neighbor grid area (CSV fil)
        /// </summary>
        public string OutMeteringGridArea_Domain_mRID { get; set; }

        public string Parent_Domain_mRID { get; set; }

        public string ServiceCategoryKind { get; set; }

        public string Technology { get; set; }
    }
}
