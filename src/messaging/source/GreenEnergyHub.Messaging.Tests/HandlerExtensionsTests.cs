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

using System.Linq;
using GreenEnergyHub.Messaging.Integration.ServiceCollection;
using GreenEnergyHub.Messaging.MessageRouting;
using GreenEnergyHub.Messaging.Tests.TestHelpers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.Extensions;
using Xunit;

namespace GreenEnergyHub.Messaging.Tests
{
    public class HandlerExtensionsTests
    {
        [Fact]
        public void HandlerExtensionsWithReasonableDefaults_Should_Setup_MediatR()
        {
            const bool validateScopes = true;
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddGreenEnergyHub();

            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes);
            var actual = serviceProvider.GetService(typeof(IMediator));

            Assert.NotNull(actual);
        }

        [Fact]
        public void HandlerExtension_Should_Locate_One_IngestionHandler()
        {
            const bool validateScopes = true;
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGreenEnergyHub(typeof(TestIngestionHandler).Assembly);

            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes);
            var messageRegistrations = serviceProvider.GetServices(typeof(MessageRegistration)).Count();

            const int expected = 4;

            Assert.Equal(expected, messageRegistrations);
        }

        [Fact]
        public void HandlerExtension_Should_inject_registrations_into_HubMessageTypeMap()
        {
            const bool validateScopes = true;
            var expectedType = typeof(TestMessage);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddGreenEnergyHub(expectedType.Assembly);

            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes);
            var messageHub = serviceProvider.GetRequiredService<IHubMessageTypeMap>();

            var actualType = messageHub.GetTypeByCategory(expectedType.Name);

            Assert.NotNull(actualType);
            Assert.Equal(expectedType, actualType);
        }
    }
}
