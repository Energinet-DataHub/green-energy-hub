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

using Energinet.DataHub.MarketData.Domain.SeedWork;

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints
{
    public class RelationshipType : EnumerationType
    {
        public static readonly RelationshipType EnergySupplier =
            new RelationshipType(0, nameof(EnergySupplier), "BRS-001");

        public static readonly RelationshipType Customer1 =
            new RelationshipType(1, nameof(Customer1), "BRS-009");

        public static readonly RelationshipType MoveOut =
            new RelationshipType(2, nameof(MoveOut), "BRS-010");

        public RelationshipType(int id, string name, string code)
            : base(id, name)
        {
            Code = code;
        }

        public string Code { get; }
    }
}
