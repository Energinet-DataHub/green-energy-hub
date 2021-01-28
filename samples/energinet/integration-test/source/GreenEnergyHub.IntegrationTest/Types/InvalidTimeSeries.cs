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

using System.Collections.Generic;

namespace GreenEnergyHub.IntegrationTest.Types
{
    public class InvalidTimeSeries
    {
        public string Id { get; set; }

        public string TimeSeries_mRID { get; set; }

        public string ProcessType { get; set; }

        public string RecipientMarketParticipant_mRID { get; set; }

        public string MarketDocument_mRID { get; set; }

        public List<ReasonDto> Reasons { get; set; }

        public string CorrelationId { get; set; }

        public string RecipientMarketParticipantMarketRole_Type { get; set; }

        public string MessageType { get; set; }
    }
}
