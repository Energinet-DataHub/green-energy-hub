using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenEnergyHub.Ingestion.Synchronous.Application;

namespace GreenEnergyHub.Ingestion.Synchronous.Tests.TestHelpers
{
    public class TestHubActionHandler : IHubActionHandler<TestActionRequest>
    {
        public Task<IHubActionResponse> Handle(TestActionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateAsync(TestActionRequest actionData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AcceptAsync(TestActionRequest actionData)
        {
            throw new NotImplementedException();
        }

        public Task<IHubActionResponse> RespondAsync(TestActionRequest actionData)
        {
            throw new NotImplementedException();
        }
    }
}
