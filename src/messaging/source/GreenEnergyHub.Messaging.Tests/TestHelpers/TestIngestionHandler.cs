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
using System.Threading;
using System.Threading.Tasks;
using GreenEnergyHub.Messaging.Dispatching;

namespace GreenEnergyHub.Messaging.Tests.TestHelpers
{
    public class TestIngestionHandler : HubRequestHandler<TestMessage>
    {
        #pragma warning disable VSTHRD200
        public Task<IHubResponse> Handle(TestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #pragma warning restore VSTHRD200

        protected override Task<bool> ValidateAsync(TestMessage actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> AcceptAsync(TestMessage actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IHubResponse> RespondAsync(TestMessage actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
