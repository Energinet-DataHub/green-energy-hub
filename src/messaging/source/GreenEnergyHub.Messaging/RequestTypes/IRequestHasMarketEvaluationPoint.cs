using GreenEnergyHub.Messaging.RequestTypes.Common;

namespace GreenEnergyHub.Messaging.RequestTypes
{
    /// <summary>
    /// An interface representing an object which has a <see cref="MarketEvaluationPoint"/> field.
    /// </summary>
    public interface IRequestHasMarketEvaluationPoint
    {
        /// <summary>
        /// The market evaluation point field
        /// </summary>
        MarketEvaluationPoint MarketEvaluationPoint { get; set; }
    }
}
