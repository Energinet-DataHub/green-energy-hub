using System;
using System.IO;
using System.Threading.Tasks;

namespace GreenEnergyHub.Messaging
{
    /// <summary>
    /// Rehydrates an object from a stream
    /// </summary>
    public interface IHubRehydrate
    {
        /// <summary>
        /// Rehydrate a message
        /// </summary>
        /// <param name="message"><see cref="Stream"/> containing the message</param>
        /// <param name="messageType">Message type to rehydrate</param>
        /// <returns>If the message type is known and the message content valid a <see cref="IHubRequest"/> else null</returns>
        Task<IHubRequest?> RehydrateAsync(Stream message, Type messageType);
    }
}
