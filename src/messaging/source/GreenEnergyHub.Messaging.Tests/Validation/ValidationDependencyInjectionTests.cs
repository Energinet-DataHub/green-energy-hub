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

using FluentAssertions;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using GreenEnergyHub.Messaging.Tests.TestHelpers.Validation;
using GreenEnergyHub.Messaging.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests.Validation
{
    [Trait("Category", "Unit")]
    public class ValidationDependencyInjectionTests
    {
        [Fact]
        public void ServiceProviderDelegateShouldBeRegistered()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGreenEnergyHub();
            var sut = serviceCollection.BuildServiceProvider();

            var serviceProviderDelegate = sut.GetService<ServiceProviderDelegate>();

            Assert.NotNull(serviceProviderDelegate);
        }

        [Fact]
        public void ServiceProviderDelegateShouldResolveRegisteredType()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGreenEnergyHub();
            serviceCollection.AddSingleton<ChangeOfSupplier>();
            var sp = serviceCollection.BuildServiceProvider();
            var sut = sp.GetService<ServiceProviderDelegate>();

            var result = sut(typeof(ChangeOfSupplier));

            result.Should()
                .NotBeNull()
                .And
                .BeOfType<ChangeOfSupplier>();
        }
    }
}
