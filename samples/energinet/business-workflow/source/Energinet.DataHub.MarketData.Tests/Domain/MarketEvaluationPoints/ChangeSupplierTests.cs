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

using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints;
using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Events;
using Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Rules.ChangeEnergySupplier;
using Energinet.DataHub.MarketData.Domain.SeedWork;
using NodaTime;
using Xunit;

namespace Energinet.DataHub.MarketData.Tests.Domain.MarketEvaluationPoints
{
    public class ChangeSupplierTests
    {
        private SystemDateTimeProviderStub _systemDateTimeProvider;

        public ChangeSupplierTests()
        {
            _systemDateTimeProvider = new SystemDateTimeProviderStub();
        }

        [Theory]
        [InlineData("exchange")]
        public void Register_WhenEvaluationPointTypeIsNotEligible_IsNotPossible(string evaluationPointTypeName)
        {
            var evaluationPointType = CreateEvaluationPointTypeFromName(evaluationPointTypeName);
            var evaluationPoint = CreateEvaluationPoint(evaluationPointType);

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is MarketEvaluationPointMustBeEnergySuppliableRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenProductionEvaluationPointIsNotObligated_IsNotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Production);

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is ProductionEvaluationPointMustBeObligatedRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenEvaluationPointIsClosedDown_IsNotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Production);
            evaluationPoint.CloseDown();

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is CannotBeInStateOfClosedDownRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenNoEnergySupplierIsAssociated_IsNotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Production);

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is MustHaveEnergySupplierAssociatedRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenChangeOfSupplierIsRegisteredOnSameDate_IsNotPossible()
        {
            var customerId = CreateCustomerId();
            var energySupplierId = CreateEnergySupplierId();
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Consumption);
            evaluationPoint.RegisterMoveIn(customerId, energySupplierId, GetFakeEffectuationDate().Minus(Duration.FromDays(1)));
            evaluationPoint.ActivateMoveIn(customerId, energySupplierId);
            evaluationPoint.RegisterChangeOfEnergySupplier(CreateEnergySupplierId(), _systemDateTimeProvider.Now(), _systemDateTimeProvider);

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is ChangeOfSupplierRegisteredOnSameDateIsNotAllowedRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenMoveInIsAlreadyRegisteredOnSameDate_IsNotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Consumption);
            evaluationPoint.RegisterMoveIn(CreateCustomerId(), CreateEnergySupplierId(), _systemDateTimeProvider.Now());

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is MoveInRegisteredOnSameDateIsNotAllowedRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenMoveOutIsAlreadyRegisteredOnSameDate_IsNotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Consumption);
            evaluationPoint.RegisterMoveIn(CreateCustomerId(), CreateEnergySupplierId(), GetFakeEffectuationDate().Minus(Duration.FromDays(1)));
            evaluationPoint.RegisterMoveOut(CreateCustomerId(), _systemDateTimeProvider.Now());

            var result = CanChangeSupplier(evaluationPoint);

            Assert.Contains(result.Rules, x => x is MoveOutRegisteredOnSameDateIsNotAllowedRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenEffectuationDateIsInThePast_NotPossible()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Consumption);
            var effectuationDate = _systemDateTimeProvider.Now().Minus(Duration.FromDays(1));

            var result = CanChangeSupplier(evaluationPoint, effectuationDate);

            Assert.Contains(result.Rules, x => x is EffectuationDateCannotBeInThePastRule && x.IsBroken);
        }

        [Fact]
        public void Register_WhenAllRulesAreSatisfied_Success()
        {
            var evaluationPoint = CreateEvaluationPoint(MarketEvaluationPointType.Consumption);
            var customerId = CreateCustomerId();
            var energySupplierId = CreateEnergySupplierId();
            evaluationPoint.RegisterMoveIn(customerId, energySupplierId, GetFakeEffectuationDate().Minus(Duration.FromDays(1)));
            evaluationPoint.ActivateMoveIn(customerId, energySupplierId);

            evaluationPoint.RegisterChangeOfEnergySupplier(CreateEnergySupplierId(), _systemDateTimeProvider.Now(), _systemDateTimeProvider);

            Assert.Contains(evaluationPoint.DomainEvents!, e => e is EnergySupplierChangeRegistered);
        }

        private static MarketParticipantMrid CreateCustomerId()
        {
            return new MarketParticipantMrid("1");
        }

        private static MarketParticipantMrid CreateEnergySupplierId()
        {
            return new MarketParticipantMrid("FakeId");
        }

        private static MarketEvaluationPoint CreateEvaluationPoint(MarketEvaluationPointType marketEvaluationPointType)
        {
            var meteringPointId = CreateEvaluationPointId();
            return new MarketEvaluationPoint(meteringPointId, marketEvaluationPointType);
        }

        private static MarketEvaluationPointMrid CreateEvaluationPointId()
        {
            return MarketEvaluationPointMrid.Create("571234567891234568");
        }

        private static MarketEvaluationPointType CreateEvaluationPointTypeFromName(string meteringPointTypeName)
        {
            return MarketEvaluationPointType.FromName<MarketEvaluationPointType>(meteringPointTypeName);
        }

        private static Instant GetFakeEffectuationDate()
        {
            return Instant.FromUtc(2000, 1, 1, 0, 0);
        }

        private BusinessRulesValidationResult CanChangeSupplier(MarketEvaluationPoint marketEvaluationPoint)
        {
            return marketEvaluationPoint.CanChangeSupplier(CreateEnergySupplierId(), _systemDateTimeProvider.Now(), _systemDateTimeProvider);
        }

        private BusinessRulesValidationResult CanChangeSupplier(MarketEvaluationPoint marketEvaluationPoint, Instant effectuationDate)
        {
            return marketEvaluationPoint.CanChangeSupplier(CreateEnergySupplierId(), effectuationDate, _systemDateTimeProvider);
        }
    }
}
