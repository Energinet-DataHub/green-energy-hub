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
using System.Text.Json.Serialization;

namespace GreenEnergyHub.Messaging.MessageTypes.Common
{
    public class Transaction
    {
        public Transaction()
            : this(Guid.NewGuid().ToString("N"))
        {
        }

        public Transaction(string mrid)
        {
            MRID = mrid;
        }

        [JsonPropertyName(name: "mRID")]
        public string MRID { get; set; }

        public static Transaction NewTransaction()
            => new Transaction(Guid.NewGuid().ToString("N"));

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Transaction)obj);
        }

        public override int GetHashCode()
        {
            return MRID.GetHashCode();
        }

        protected bool Equals(Transaction other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return MRID == other.MRID;
        }
    }
}
