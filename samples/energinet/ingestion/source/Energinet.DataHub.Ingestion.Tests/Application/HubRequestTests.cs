﻿// Copyright 2020 Energinet DataHub A/S
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
using System.Linq;
using Energinet.DataHub.Ingestion.Application.ChangeOfSupplier;
using FluentAssertions;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Messaging.MessageTypes;
using Xunit;

namespace Energinet.DataHub.Ingestion.Tests.Application
{
    public class HubRequestTests
    {
        [Fact]
        public void Must_implement_request_queue_attribute()
        {
            var assembly = typeof(ChangeOfSupplierRequestHandler).Assembly;
            var actionRequests = assembly.GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IHubMessage)))
                .ToList();

            actionRequests.ForEach(request =>
            {
                var attribute = Attribute.GetCustomAttribute(request, typeof(HubMessageQueueAttribute));
                attribute.Should().NotBeNull("Hub requests must implement HubMessageQueueAttribute");
            });
        }
    }
}
