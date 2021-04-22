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
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GreenEnergyHub.Conversion.CIM.Components;

namespace GreenEnergyHub.Conversion.CIM.Json
{
    public class JsonWriterFactory
    {
        private static readonly Dictionary<Type, Func<JsonPayloadWriter>> _writers;
        private static readonly JsonWriterFactory _instance;

        static JsonWriterFactory()
        {
            _writers = new Dictionary<Type, Func<JsonPayloadWriter>>
            {
                { typeof(RequestChangeOfPriceList), () => new RequestChangeOfPriceListWriter() },
            };
            _instance = new JsonWriterFactory();
        }

        protected JsonWriterFactory() { }

        public static JsonDocumentWriter CreateWriter(Type typeOfPayload, Stream outputStream)
        {
            return _instance.Create(typeOfPayload, outputStream);
        }

        public static JsonDocumentWriter<TPayload> CreateWriter<TPayload>(Stream outputStream)
            where TPayload : MktActivityRecord
        {
            return _instance.Create<TPayload>(outputStream);
        }

        public JsonDocumentWriter<TPayload> Create<TPayload>(Stream outputStream)
            where TPayload : MktActivityRecord
        {
            var payloadType = typeof(TPayload);
            var jsonWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
            if (!CanResolveWriter(payloadType)) throw new ArgumentException("No writer defined for payload type");

            var payloadWriter = ResolveWriter(payloadType);
            return new JsonDocumentWriter<TPayload>(jsonWriter, payloadWriter);
        }

        public JsonDocumentWriter Create(Type payloadType, Stream outputStream)
        {
            var jsonWriter = new Utf8JsonWriter(outputStream, new JsonWriterOptions { Indented = true });
            if (!CanResolveWriter(payloadType)) throw new ArgumentException("No writer defined for payload type");

            var payloadWriter = ResolveWriter(payloadType);
            return new JsonDocumentWriter(jsonWriter, payloadWriter);
        }

        protected virtual JsonPayloadWriter ResolveWriter(Type payloadType)
        {
            return _writers[payloadType].Invoke();
        }

        protected virtual bool CanResolveWriter(Type payloadType)
        {
            return _writers.ContainsKey(payloadType);
        }
    }
}
