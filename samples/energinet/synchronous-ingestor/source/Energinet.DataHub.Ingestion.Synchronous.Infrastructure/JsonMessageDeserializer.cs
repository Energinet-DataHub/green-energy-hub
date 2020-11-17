using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using GreenEnergyHub.Ingestion;
using Microsoft.Extensions.Logging;

namespace Energinet.DataHub.Ingestion.Synchronous.Infrastructure
{
    public class JsonMessageDeserializer : IHubRehydrate
    {
        private readonly ILogger<JsonMessageDeserializer> _logger;

        public JsonMessageDeserializer(
            ILogger<JsonMessageDeserializer> logger)
        {
            _logger = logger;
        }

        public async Task<IHubActionRequest?> RehydrateAsync(Stream message, Type messageType)
        {
            try
            {
                var request = await JsonSerializer.DeserializeAsync(message, messageType).ConfigureAwait(false);
                return request as IHubActionRequest;
            }
            #pragma warning disable CA1031
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to rehydrate message");
            }
            #pragma warning restore CA1031

            return null;
        }
    }
}
