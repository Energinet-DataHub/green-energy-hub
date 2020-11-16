using System;
using System.Text.Json.Serialization;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RequestTypes.Common
{
    public class MarketParticipant
    {
        public MarketParticipant()
            : this(MRid.Empty, null) { }

        public MarketParticipant(string mrid)
            : this(new MRid(mrid), null)
        { }

        public MarketParticipant(MRid mRid, string? name)
        {
            MRid = mRid;
            Name = name;
        }

        public static MarketParticipant Empty => new MarketParticipant();

        [JsonPropertyName(name: "mRID")]
        public MRid MRid { get; set; }

        [JsonPropertyName(name: "name")]
        public string? Name { get; set; }

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

            return obj.GetType() == typeof(MarketParticipant) && Equals((MarketParticipant)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MRid, Name);
        }

        protected bool Equals(MarketParticipant other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return MRid.Equals(other.MRid) && Name == other.Name;
        }
    }
}
