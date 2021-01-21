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

using System.Reflection;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using GreenEnergyHub.Messaging.Tests.TestHelpers.Validation;
using GreenEnergyHub.Messaging.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    [Trait("Category", "Unit")]
    public class ValidationAssemblyScanningTests
    {
        [Fact]
        public void AssemblyScanningShouldFindGenericRuleSetDefinition()
        {
            var services = new ServiceCollection();
            var targetAssembly = typeof(ChangeSupplierRuleCollection).Assembly;
            services.AddGreenEnergyHub(targetAssembly);
            var sut = services.BuildServiceProvider();

            var result = sut.GetService<RuleCollection<ChangeOfSupplier>>();

            Assert.NotNull(result);
        }

        [Fact]
        public void AssemblyScanningShouldFindGenericPropertyRule()
        {
            var services = new ServiceCollection();
            var targetAssembly = typeof(MarketEvaluationPointValidation).Assembly;
            services.AddGreenEnergyHub(targetAssembly);
            var sut = services.BuildServiceProvider();

            var result = sut.GetService<MarketEvaluationPointValidation>();

            Assert.NotNull(result);
        }

        [Fact]
        public void AssemblyScanningShouldConfigureRuleEngine()
        {
            var services = new ServiceCollection();
            services.AddGreenEnergyHub(Assembly.GetExecutingAssembly());
            var sut = services.BuildServiceProvider();

            var result = sut.GetService<IRuleEngine<ChangeOfSupplier>>();

            Assert.NotNull(result);
        }
    }
}
