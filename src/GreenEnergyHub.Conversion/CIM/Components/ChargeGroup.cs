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

namespace GreenEnergyHub.Conversion.CIM.Components
{
    public class ChargeGroup
    {
        private readonly List<ChargeType> _chargeTypes;

        public ChargeGroup()
        {
            _chargeTypes = new List<ChargeType>();
        }

        public IReadOnlyCollection<ChargeType> ChargeTypes => _chargeTypes;

        public void Add(ChargeType chargeType)
        {
            _chargeTypes.Add(chargeType);
        }

        public void AddRange(ICollection<ChargeType> chargeTypes)
        {
            _chargeTypes.AddRange(chargeTypes);
        }
    }
}
