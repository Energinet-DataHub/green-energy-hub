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
using System.Text.Json;
using GreenEnergyHub.Conversion.CIM.Components;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    /// <summary>
    /// Write a payload as CIM
    /// </summary>
    /// <typeparam name="TPayload">Payload type to write</typeparam>
    public abstract class JsonPayloadWriter<TPayload> : JsonPayloadWriter
        where TPayload : MktActivityRecord
    {
        internal override void WritePayload(Utf8JsonWriter writer, MktActivityRecord record)
        {
            if (record is not TPayload payload) throw new ArgumentException("Input does not match record type");

            WritePayload(writer, payload);
        }

        /// <summary>
        /// Write the payload
        /// </summary>
        /// <param name="writer">Json writer to use</param>
        /// <param name="payload">Payload to write</param>
        protected abstract void WritePayload(Utf8JsonWriter writer, TPayload payload);
    }
}
