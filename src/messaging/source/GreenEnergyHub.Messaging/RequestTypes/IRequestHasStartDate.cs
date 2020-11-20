using System;

namespace GreenEnergyHub.Messaging.RequestTypes
{
    /// <summary>
    /// An interface representing an object which has a <see cref="StartDate"/> field.
    /// </summary>
    public interface IRequestHasStartDate
    {
        /// <summary>
        /// The start date field
        /// </summary>
        DateTime StartDate { get; set; }
    }
}
