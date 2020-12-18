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

using System;
using Confluent.Kafka;

namespace GreenEnergyHub.Queues.Kafka
{
    public class KafkaProducerFactory : IKafkaProducerFactory
    {
        private readonly KafkaConfiguration _kafkaConfiguration;

        public KafkaProducerFactory(KafkaConfiguration kafkaConfiguration)
        {
            _kafkaConfiguration = kafkaConfiguration ?? throw new ArgumentNullException(nameof(kafkaConfiguration));
        }

        public IProducer<Null, string> Build()
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = _kafkaConfiguration.BoostrapServers,
                SecurityProtocol = Enum.Parse<SecurityProtocol>(_kafkaConfiguration.SecurityProtocol, true),
                SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaConfiguration.SaslMechanism, true),
                SaslUsername = _kafkaConfiguration.SaslUsername,
                SaslPassword = _kafkaConfiguration.SaslPassword,
                MessageSendMaxRetries = _kafkaConfiguration.MessageSendMaxRetries,
                MessageTimeoutMs = _kafkaConfiguration.MessageTimeoutMs,
                SslCaLocation = _kafkaConfiguration.SslCaLocation,
            };

            return new ProducerBuilder<Null, string>(producerConfig).Build();
        }
    }
}
