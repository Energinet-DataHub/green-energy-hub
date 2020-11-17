using GreenEnergyHub.Ingestion.RequestTypes.Common;

namespace GreenEnergyHub.Ingestion.RequestTypes
{
    /// <summary>
    /// An interface representing an object which has a <see cref="BalanceResponsibleParty"/> field.
    /// </summary>
    public interface IRequestHasBalanceResponsibleParty
    {
        /// <summary>
        /// The balance responsible party field
        /// </summary>
        MarketParticipant BalanceResponsibleParty { get; set; }
    }
}
