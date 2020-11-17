using System.Collections.Generic;

namespace GreenEnergyHub.Ingestion
{
    /// <summary>
    /// Represents a response from Green Energy Hub
    /// </summary>
    public interface IHubActionResponse
    {
        /// <summary>
        /// Whether the action was successful.
        /// </summary>
        /// <value>True if the action was successful. Defaults to false.</value>
        bool IsSuccessful { get; set; }

        /// <summary>
        /// A list of any errors encountered.
        /// </summary>
        /// <value>The list of errors. Empty by default.</value>
        List<string> Errors { get; }
    }
}
