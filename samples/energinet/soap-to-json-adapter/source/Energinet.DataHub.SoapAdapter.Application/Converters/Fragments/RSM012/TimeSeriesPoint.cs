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
using NodaTime;

namespace Energinet.DataHub.SoapAdapter.Application.Converters.Fragments.RSM012
{
    public class TimeSeriesPoint
    {
        private const string QuantityQualityMissing = "A02";

        public TimeSeriesPoint()
        {
            Position = 0;
            EnergyQuantity = 0m;
            QuantityMissing = false;
            QuantityQuality = QuantityQualityMissing;
        }

        public int Position { get; set; }

        public decimal EnergyQuantity { get; set; }

        public bool QuantityMissing { get; set; }

        public string QuantityQuality { get; set; }

        public decimal GetCimEnergyQuantity()
        {
            return QuantityMissing ? 0.0m : EnergyQuantity;
        }

        public string GetCimQuantityQuality()
        {
            return QuantityMissing ? QuantityQualityMissing : QuantityQuality;
        }
    }
}
