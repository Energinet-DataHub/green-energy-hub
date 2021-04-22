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
using GreenEnergyHub.Conversion.CIM.Values;

namespace GreenEnergyHub.Conversion.CIM.Components
{
    public class ChargeType
    {
        private readonly List<SeriesPeriodTimeframe> _seriesPeriods;

        public ChargeType(PartyId partyId, ChargeKind? chargeKind = null, string? description = null, bool? transparentInvoicing = null, bool? taxIndicator = null, DateTime? terminationDate = null, string? mRid = null, VatPayerKind? vatPayer = null, string? name = null)
        {
            PartyId = partyId ?? throw new ArgumentNullException(nameof(partyId));
            ChargeKind = chargeKind;
            Description = description;
            TransparentInvoicing = transparentInvoicing;
            TaxIndicator = taxIndicator;
            TerminationDate = terminationDate;
            MRid = mRid;
            VatPayer = vatPayer;
            Name = name;
            _seriesPeriods = new List<SeriesPeriodTimeframe>();
        }

        public PartyId PartyId { get; }

        public ChargeKind? ChargeKind { get; }

        public string? Description { get; }

        public bool? TransparentInvoicing { get; }

        public bool? TaxIndicator { get; }

        public DateTime? TerminationDate { get; }

        public string? MRid { get; }

        public VatPayerKind? VatPayer { get; }

        public string? Name { get; }

        public IReadOnlyCollection<SeriesPeriodTimeframe> SeriesPeriods => _seriesPeriods;

        public void Add(SeriesPeriodTimeframe period) => _seriesPeriods.Add(period);
    }
}
