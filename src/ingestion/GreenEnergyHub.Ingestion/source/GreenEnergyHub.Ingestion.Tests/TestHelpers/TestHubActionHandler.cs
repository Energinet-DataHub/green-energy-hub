using System;
using System.Threading;
using System.Threading.Tasks;

namespace GreenEnergyHub.Ingestion.Tests.TestHelpers
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
