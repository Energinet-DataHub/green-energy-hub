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

namespace GreenEnergyHub.Messaging.RequestTypes.Common
{
    public class MRid
    {
        public MRid()
            : this(string.Empty) { }

        public MRid(string value, string? qualifier = null)
        {
            Value = value;
            Qualifier = qualifier;
        }

        public static MRid Empty => new MRid();

        [JsonPropertyName(name: "value")]
        public string Value { get; set;  }

        [JsonPropertyName(name: "qualifier")]
        public string? Qualifier { get; set; }

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

            return obj.GetType() == typeof(MRid) && Equals((MRid)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Qualifier);
        }

        protected bool Equals(MRid other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return Value == other.Value && Qualifier == other.Qualifier;
        }
    }
}
