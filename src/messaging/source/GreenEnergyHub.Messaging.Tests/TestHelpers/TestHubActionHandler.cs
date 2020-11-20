using System;
using System.Threading;
using System.Threading.Tasks;
using GreenEnergyHub.Messaging.Dispatching;

namespace GreenEnergyHub.Messaging.Tests.TestHelpers
{
    public class TestIngestionHandler : HubRequestHandler<TestActionRequest>
    {
        #pragma warning disable VSTHRD200
        public Task<IHubResponse> Handle(TestActionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #pragma warning restore VSTHRD200

        protected override Task<bool> ValidateAsync(TestActionRequest actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> AcceptAsync(TestActionRequest actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override Task<IHubResponse> RespondAsync(TestActionRequest actionData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
