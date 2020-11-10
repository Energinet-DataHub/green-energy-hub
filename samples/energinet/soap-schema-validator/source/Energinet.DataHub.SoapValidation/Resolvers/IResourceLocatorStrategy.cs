using System;

namespace Energinet.DataHub.SoapValidation.Resolvers
{
    internal interface IResourceLocatorStrategy
    {
        /// <summary>
        /// Get resource path
        /// </summary>
        /// <param name="absoluteUri">Resource to map</param>
        /// <returns>Path to mapped resource</returns>
        string GetResourcePath(Uri absoluteUri);
    }
}