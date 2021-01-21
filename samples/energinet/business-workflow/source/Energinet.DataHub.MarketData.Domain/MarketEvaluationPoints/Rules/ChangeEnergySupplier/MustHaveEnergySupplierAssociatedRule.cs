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
using System.Linq;
using Energinet.DataHub.MarketData.Domain.SeedWork;

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Rules.ChangeEnergySupplier
{
    public class MustHaveEnergySupplierAssociatedRule : IBusinessRule
    {
        private readonly IReadOnlyList<Relationship> _relationships;

        public MustHaveEnergySupplierAssociatedRule(IReadOnlyList<Relationship> relationships)
        {
            _relationships = relationships;
        }

        public bool IsBroken => !HasActiveEnergySupplier();

        public string Message => $"Metering point must have an energy supplier associated.";

        private bool HasActiveEnergySupplier()
        {
            return _relationships.Any(r =>
                r.Type.Equals(RelationshipType.EnergySupplier) && r.State.Equals(RelationshipState.Active));
        }
    }
}
