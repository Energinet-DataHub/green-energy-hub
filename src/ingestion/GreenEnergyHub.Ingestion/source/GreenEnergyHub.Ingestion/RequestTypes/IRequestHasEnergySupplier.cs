using GreenEnergyHub.Ingestion.RequestTypes.Common;

namespace GreenEnergyHub.Ingestion.RequestTypes
{
    /// <summary>
    /// An interface representing an object which has a energy supplier field.
    /// </summary>
    public interface IRequestHasEnergySupplier
    {
        /// <summary>
        /// The energy supplier field
        /// </summary>
        MarketParticipant EnergySupplier { get; set; }
    }
}
