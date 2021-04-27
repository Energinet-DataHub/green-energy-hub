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
using System.Threading.Tasks;
using System.Xml;
using GreenEnergyHub.Conversion.CIM;
using GreenEnergyHub.Conversion.CIM.Json;
using GreenEnergyHub.DkEbix.Parsers;

namespace GreenEnergyHub.DkEbix
{
    /// <summary>
    /// Convert Ebix xml to CIM json
    /// </summary>
    public sealed class ConversionService
    {
        private static readonly Dictionary<Type, Type> _conversionMap = new ()
        {
            { typeof(Rsm033), typeof(RequestChangeOfPriceList) },
        };

        private readonly XmlReaderSettings _xmlReaderSettings;

        public ConversionService()
        {
            _xmlReaderSettings = new XmlReaderSettings { Async = true };
        }

        /// <summary>
        /// Reads an Ebix xml stream from <paramref name="inputStream"/> and write the CIM json to the <paramref name="outputStream"/>
        /// </summary>
        /// <param name="inputStream">Readable input stream</param>
        /// <param name="outputStream">Writable output stream</param>
        /// <exception cref="ArgumentNullException"><paramref name="inputStream"/> or <paramref name="outputStream"/> is null</exception>
        /// <exception cref="ResolveRsmParserException">No parser is found for the input stream</exception>
        public async Task ConvertStreamAsync(Stream inputStream, Stream outputStream)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));

            using var reader = XmlReader.Create(inputStream, _xmlReaderSettings);
            var parser = await RsmParserFactory.CreateParserAsync(reader);

            if (!ContainsParser(parser)) throw new ResolveRsmParserException("Unsupported parser");

            await using var writer = JsonWriterFactory.CreateWriter(GetPayloadWriterType(parser), outputStream);

            writer.WriteDocument(await parser.ReadMarketDocumentAsync(reader));

            await foreach (var payload in parser.ReadPayloadsAsync(reader))
            {
                if (payload == null) break;
                writer.WritePayload(payload);
            }

            await writer.CloseAsync();
        }

        internal static bool ContainsParser(Type parserType) => _conversionMap.ContainsKey(parserType);

        private static bool ContainsParser(RsmParser? parser)
        {
            return parser != null && ContainsParser(parser.GetType());
        }

        private static Type GetPayloadWriterType(RsmParser parser)
        {
            return _conversionMap[parser.GetType()];
        }
    }
}
