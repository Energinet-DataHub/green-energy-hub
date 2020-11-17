using System;
using System.Text.Json.Serialization;

namespace GreenEnergyHub.Ingestion.RequestTypes.Common
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
