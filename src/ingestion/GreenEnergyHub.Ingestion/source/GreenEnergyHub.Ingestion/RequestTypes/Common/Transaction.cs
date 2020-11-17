using System;
using System.Text.Json.Serialization;

namespace GreenEnergyHub.Ingestion.RequestTypes.Common
{
    public class Transaction
    {
        public Transaction()
            : this(Guid.NewGuid().ToString("N"))
        {
        }

        public Transaction(string mRid)
        {
            MRid = mRid;
        }

        [JsonPropertyName(name: "mRID")]
        public string MRid { get; set; }

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
            return MRid.GetHashCode();
        }

        protected bool Equals(Transaction other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return MRid == other.MRid;
        }
    }
}
