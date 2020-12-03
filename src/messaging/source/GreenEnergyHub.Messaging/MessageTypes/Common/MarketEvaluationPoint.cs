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
    public class MarketEvaluationPoint
    {
        public MarketEvaluationPoint()
        : this(string.Empty)
        {
        }

        public MarketEvaluationPoint(string mrid)
        {
            MRid = mrid;
        }

        public static MarketEvaluationPoint Empty => new MarketEvaluationPoint();

        [JsonPropertyName(name: "mRID")]
        public string MRid { get; set; }

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

            return obj.GetType() == GetType() && Equals((MarketEvaluationPoint)obj);
        }

        public override int GetHashCode()
        {
            return MRid.GetHashCode();
        }

        protected bool Equals(MarketEvaluationPoint other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return MRid == other.MRid;
        }
    }
}
