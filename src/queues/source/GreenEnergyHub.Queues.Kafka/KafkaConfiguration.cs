// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace GreenEnergyHub.Queues.Kafka
{
    /// <summary>
    /// Configuration settings used for building a kafka producer.
    /// </summary>
    public class KafkaConfiguration
    {
        /// <summary>
        /// Kafka bootstrap servers list
        /// </summary>
        public string BoostrapServers { get; set; } = string.Empty;

        /// <summary>
        /// The security protocol to use. Possible values are: Plaintext, Ssl, SaslPlaintext, SaslSsl.
        /// </summary>
        public string SecurityProtocol { get; set; } = string.Empty;

        /// <summary>
        /// SaslMechanism. Possible values are: Gssapi, Plain, ScramSha256, ScramSha512, OAuthBearer.
        /// </summary>
        public string SaslMechanism { get; set; } = string.Empty;

        /// <summary>
        /// SaslUsername to use. For Azure Event Hubs use: '$ConnectionString'
        /// </summary>
        public string SaslUsername { get; set; } = string.Empty;

        /// <summary>
        /// SaslPassword to use. For Azure Event Hubs use the connection string.
        /// </summary>
        public string SaslPassword { get; set; } = string.Empty;

        /// <summary>
        /// Maximum number of retries.
        /// </summary>
        public int MessageSendMaxRetries { get; set; }

        /// <summary>
        /// Timeout in ms.
        /// </summary>
        public int MessageTimeoutMs { get; set; }

        /// <summary>
        /// Location of a .pem file containing Certificate authorities.
        /// </summary>
        public string SslCaLocation { get; set; } = string.Empty;
    }
}
