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
using System.Linq;
using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Events;
using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Rules.ChangeEnergySupplier;
using Energinet.DataHub.MarketData.Domain.SeedWork;
using NodaTime;

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints
{
    public sealed class MarketEvaluationPoint : Entity
    {
        private readonly MarketEvaluationPointType _marketEvaluationPointType;
        private readonly MarketEvaluationPointMrid _marketEvaluationPointMrid;
        private bool _isProductionObligated;
        private PhysicalState _physicalState;
        private List<Relationship> _relationships = new List<Relationship>();

        public MarketEvaluationPoint(MarketEvaluationPointMrid marketEvaluationPointMrid, MarketEvaluationPointType marketEvaluationPointType)
        {
            _marketEvaluationPointMrid = marketEvaluationPointMrid;
            _marketEvaluationPointType = marketEvaluationPointType;
            _physicalState = PhysicalState.New;
            AddDomainEvent(new MeteringPointCreated(_marketEvaluationPointMrid, _marketEvaluationPointType));
        }

        private MarketEvaluationPoint(MarketEvaluationPointMrid marketEvaluationPointMrid, MarketEvaluationPointType marketEvaluationPointType, bool isProductionObligated)
            : this(marketEvaluationPointMrid, marketEvaluationPointType)
        {
            _isProductionObligated = isProductionObligated;
        }

        public static MarketEvaluationPoint CreateProduction(MarketEvaluationPointMrid marketEvaluationPointMrid, bool isObligated)
        {
            return new MarketEvaluationPoint(marketEvaluationPointMrid, MarketEvaluationPointType.Production, isObligated);
        }

        public BusinessRulesValidationResult CanChangeSupplier(MarketParticipantMrid energySupplierMrid, Instant effectuationDate, ISystemDateTimeProvider systemDateTimeProvider)
        {
            if (energySupplierMrid is null)
            {
                throw new ArgumentNullException(nameof(energySupplierMrid));
            }

            if (systemDateTimeProvider == null)
            {
                throw new ArgumentNullException(nameof(systemDateTimeProvider));
            }

            var rules = new List<IBusinessRule>()
            {
                new MarketEvaluationPointMustBeEnergySuppliableRule(_marketEvaluationPointType),
                new ProductionEvaluationPointMustBeObligatedRule(_marketEvaluationPointType, _isProductionObligated),
                new CannotBeInStateOfClosedDownRule(_physicalState),
                new MustHaveEnergySupplierAssociatedRule(_relationships.AsReadOnly()),
                new ChangeOfSupplierRegisteredOnSameDateIsNotAllowedRule(_relationships.AsReadOnly(), effectuationDate),
                new MoveInRegisteredOnSameDateIsNotAllowedRule(_relationships.AsReadOnly(), effectuationDate),
                new MoveOutRegisteredOnSameDateIsNotAllowedRule(_relationships.AsReadOnly(), effectuationDate),
                new EffectuationDateCannotBeInThePastRule(effectuationDate, systemDateTimeProvider.Now()),
            };

            return new BusinessRulesValidationResult(rules);
        }

        public void RegisterChangeOfEnergySupplier(MarketParticipantMrid energySupplierMrid, Instant effectuationDate, ISystemDateTimeProvider systemDateTimeProvider)
        {
            if (CanChangeSupplier(energySupplierMrid, effectuationDate, systemDateTimeProvider).AreAnyBroken == true)
            {
                throw new InvalidOperationException();
            }

            _relationships.Add(new Relationship(energySupplierMrid,  RelationshipType.EnergySupplier, effectuationDate));
            AddDomainEvent(new EnergySupplierChangeRegistered(_marketEvaluationPointMrid, energySupplierMrid, effectuationDate));
        }

        public void CloseDown()
        {
            if (_physicalState != PhysicalState.ClosedDown)
            {
                _physicalState = PhysicalState.ClosedDown;
                AddDomainEvent(new MeteringPointClosedDown(_marketEvaluationPointMrid));
            }
        }

        public void RegisterMoveIn(MarketParticipantMrid customerMrid, MarketParticipantMrid energySupplierMrid, Instant effectuationDate)
        {
            if (customerMrid is null)
            {
                throw new ArgumentNullException(nameof(customerMrid));
            }

            if (energySupplierMrid is null)
            {
                throw new ArgumentNullException(nameof(energySupplierMrid));
            }

            _relationships.Add(new Relationship(customerMrid, RelationshipType.Customer1, effectuationDate));
            _relationships.Add(new Relationship(energySupplierMrid, RelationshipType.EnergySupplier, effectuationDate));
        }

        public void ActivateMoveIn(MarketParticipantMrid customerMrid, MarketParticipantMrid energySupplierMrid)
        {
            if (customerMrid is null)
            {
                throw new ArgumentNullException(nameof(customerMrid));
            }

            if (energySupplierMrid is null)
            {
                throw new ArgumentNullException(nameof(energySupplierMrid));
            }

            var customerRelation = _relationships.First(r =>
                r.ParticipantMrid.Equals(customerMrid) && r.Type == RelationshipType.Customer1);
            customerRelation.Activate();

            var energySupplierRelation = _relationships.First(r =>
                r.ParticipantMrid.Equals(energySupplierMrid) && r.Type == RelationshipType.EnergySupplier);
            energySupplierRelation.Activate();
        }

        public void RegisterMoveOut(MarketParticipantMrid customerMrid, Instant effectuationDate)
        {
            if (customerMrid is null)
            {
                throw new ArgumentNullException(nameof(customerMrid));
            }

            _relationships.Add(new Relationship(customerMrid, RelationshipType.MoveOut, effectuationDate));
        }
    }
}
