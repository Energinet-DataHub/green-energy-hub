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

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Rules.ChangeEnergySupplier
{
    public class MarketEvaluationPointMustBeEnergySuppliableRule : IBusinessRule
    {
        private readonly MarketEvaluationPointType _marketEvaluationPointType;

        public MarketEvaluationPointMustBeEnergySuppliableRule(MarketEvaluationPointType marketEvaluationPointType)
        {
            _marketEvaluationPointType = marketEvaluationPointType;
        }

        public bool IsBroken => !(_marketEvaluationPointType == MarketEvaluationPointType.Production ||
                                  _marketEvaluationPointType == MarketEvaluationPointType.Consumption);

        public string Message => $"Market evaluation point is eligible of having energy supplier associated.";
    }
}
