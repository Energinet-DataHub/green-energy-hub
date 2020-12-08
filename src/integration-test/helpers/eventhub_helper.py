# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
from azure.eventhub.aio import EventHubProducerClient
from azure.eventhub import EventData
import asyncio


async def insert_content_on_eventhub(eventhub_conn_str, content):
    conn_str = eventhub_conn_str
    producer = EventHubProducerClient.from_connection_string(conn_str=conn_str)
    async with producer:
        event_data_batch = await producer.create_batch()
        event_data_batch.add(EventData(content))
        await producer.send_batch(event_data_batch)
