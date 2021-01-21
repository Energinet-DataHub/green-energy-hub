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
using System.Runtime.Serialization;

namespace Energinet.DataHub.MarketData.Domain.SeedWork
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException()
        {
        }

        public BusinessRuleException(string? message)
            : base(message)
        {
        }

        public BusinessRuleException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected BusinessRuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
