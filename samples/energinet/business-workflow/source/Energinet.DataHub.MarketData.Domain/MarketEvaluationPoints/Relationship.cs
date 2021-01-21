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

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints
{
    public class Relationship
    {
        public Relationship(MarketParticipantMrid marketParticipantMrid, RelationshipType type, Instant effectuationDate)
        {
            ParticipantMrid = marketParticipantMrid ?? throw new ArgumentNullException(nameof(marketParticipantMrid));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            EffectuationDate = effectuationDate;
            State = RelationshipState.Pending;
        }

        public MarketParticipantMrid ParticipantMrid { get; }

        public RelationshipType Type { get; }

        public Instant EffectuationDate { get; }

        public RelationshipState State { get; private set; }

        public void Activate()
        {
            if (State != RelationshipState.Active)
            {
                State = RelationshipState.Active;
            }
        }
    }
}
