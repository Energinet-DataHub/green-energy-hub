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

using NodaTime;

namespace GreenEnergyHub.IntegrationTest.Types
{
    public class ParquetTimeSeries
    {
        public string MarketEvaluationPointMRID { get; set; }

        public Instant Time { get; set; }

        public decimal Quantity { get; set; }

        public string CorrelationId { get; set; }

        public string MessageReference { get; set; }

        public string MarketDocumentMRID { get; set; }

        public Instant CreatedTime { get; set; }

        public string SenderMarketParticipantMRID { get; set; }

        public string ProcessType { get; set; }

        public string SenderMarketParticipantMarketRoleType { get; set; }

        public string TimeSeriesMRID { get; set; }

        public string MktActivityRecordStatus { get; set; }

        public string MarketEvaluationPointType { get; set; }

        public string Quality { get; set; }

        public string MeterReadingPeriodicity { get; set; }

        public string MeteringMethod { get; set; }

        public string MeteringGridAreaDomainMRID { get; set; }

        public string ConnectionState { get; set; }

        public string EnergySupplierMarketParticipantMRID { get; set; }

        public string BalanceResponsiblePartyMarketParticipantMRID { get; set; }

        public string InMeteringGridAreaDomainMRID { get; set; }

        public string OutMeteringGridAreaDomainMRID { get; set; }

        public string ParentDomainMRID { get; set; }

        public string ServiceCategoryKind { get; set; }

        public string SettlementMethod { get; set; }

        public string QuantityMeasurementUnitName { get; set; }

        public string Product { get; set; }
    }
}
