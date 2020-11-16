using System;
using System.Text.Json.Serialization;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestTypes.Common
{
    public class MarketEvaluationPoint
    {
        public MarketEvaluationPoint()
        : this(string.Empty)
        {
        }

        public MarketEvaluationPoint(string mRid)
        {
            MRid = mRid;
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
