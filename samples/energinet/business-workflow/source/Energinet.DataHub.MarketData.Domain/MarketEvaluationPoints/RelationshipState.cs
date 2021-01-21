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
    public class RelationshipState : EnumerationType
    {
        public static readonly RelationshipState Pending = new RelationshipState(0, nameof(Pending));
        public static readonly RelationshipState Active = new RelationshipState(1, nameof(Active));

        public RelationshipState(int id, string name)
            : base(id, name)
        {
        }
    }
}
