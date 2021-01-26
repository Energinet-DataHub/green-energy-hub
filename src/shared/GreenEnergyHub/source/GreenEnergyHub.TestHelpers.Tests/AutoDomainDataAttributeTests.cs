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

using AutoFixture.Xunit2;
using GreenEnergyHub.TestHelpers.Traits;
using Xunit;

namespace GreenEnergyHub.TestHelpers.Tests
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class AutoDomainDataAttributeTests
    {
        [Theory]
        [AutoDomainData]
        public void CreatesNonDefaultData(string x)
        {
            Assert.NotEqual(default, x);
        }

        [Theory]
        [AutoDomainData]
        public void CreatesNonNullData(string x)
        {
            Assert.NotNull(x);
        }

        [Theory]
        [AutoDomainData]
        public void UsesFrozenArgumentAsDependency([Frozen] string frozenString, StringOwner dependee)
        {
            Assert.Equal(frozenString, dependee.CtorInjectedString);
        }

        // ReSharper disable once CA1034
        public class StringOwner
        {
            public StringOwner(string foo)
            {
                CtorInjectedString = foo;
            }

            public string CtorInjectedString { get; }
        }
    }
}
