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
using System.Threading.Tasks;
using GreenEnergyHub.Json;
using GreenEnergyHub.Messaging;
using GreenEnergyHub.Queues.Kafka;

namespace GreenEnergyHub.Queues.ValidationReportDispatcher
{
    public class ValidationReportQueueDispatcher : IValidationReportQueueDispatcher
    {
        private readonly IKafkaDispatcher _kafkaDispatcher;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly string _topic;

        public ValidationReportQueueDispatcher(
            IKafkaDispatcher kafkaDispatcher,
            IJsonSerializer jsonSerializer,
            string topic)
        {
            _kafkaDispatcher = kafkaDispatcher ?? throw new ArgumentNullException(nameof(kafkaDispatcher));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _topic = !string.IsNullOrEmpty(topic) ? topic : throw new ArgumentNullException(nameof(topic));
        }

        public Task DispatchAsync(IHubMessage hubMessage)
        {
            if (hubMessage == null)
            {
                throw new ArgumentNullException(nameof(hubMessage));
            }

            return _kafkaDispatcher.DispatchAsync(_jsonSerializer.Serialize(hubMessage), _topic);
        }
    }
}
