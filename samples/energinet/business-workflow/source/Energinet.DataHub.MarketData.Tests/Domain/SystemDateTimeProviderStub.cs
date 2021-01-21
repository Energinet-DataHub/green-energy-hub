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

using System.Runtime.InteropServices.ComTypes;
using Energinet.DataHub.MarketData.Domain;
using Energinet.DataHub.MarketData.Domain.SeedWork;
using NodaTime;

namespace Energinet.DataHub.MarketData.Tests.Domain
{
    public class SystemDateTimeProviderStub : ISystemDateTimeProvider
    {
        private Instant _now = SystemClock.Instance.GetCurrentInstant();

        public void SetNow(Instant now)
        {
            _now = now;
        }

        public Instant Now()
        {
            return _now;
        }
    }
}
