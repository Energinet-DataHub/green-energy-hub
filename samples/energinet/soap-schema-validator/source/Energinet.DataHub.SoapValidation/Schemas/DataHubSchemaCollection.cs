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
using System.Collections.Generic;
using Energinet.DataHub.SoapValidation.Dtos;
using Energinet.DataHub.SoapValidation.Resolvers;

namespace Energinet.DataHub.SoapValidation.Schemas
{
    internal class DataHubSchemaCollection : SchemaCollection
    {
        private static readonly SchemaDefinition[] Definitions =
        {
            CreateDefinition(
                "Envelope",
                "http://schemas.xmlsoap.org/soap/envelope/",
                "soap-envolope-manual.xsd",
                "DataHub",
                new List<SchemaDefinition> { CreateSendMessageRequestDefinition(), CreatePeekMessageResponseDefinition() }),
        };

        public DataHubSchemaCollection()
            : base(Definitions)
        {
        }

        private static SchemaDefinition CreatePeekMessageResponseDefinition()
        {
            var acknowledgementDefinition = CreateDefinition(
                "DK_Acknowledgement",
                "un:unece:260:data:EEM-DK_Acknowledgement:v3",
                "ebIX_DK_Acknowledgement-2.xsd",
                "DK_Acknowledgement");
            return CreateDefinition(
                "PeekMessageResponse",
                "urn:www:datahub:dk:b2b:v01",
                "DataHUBB2B-manual.xsd",
                "DataHub",
                new List<SchemaDefinition> { acknowledgementDefinition });
        }

        private static SchemaDefinition CreateSendMessageRequestDefinition()
        {
            var meteredDataTimeSeriesDefinition = CreateDefinition(
                "DK_MeteredDataTimeSeries",
                "un:unece:260:data:EEM-DK_MeteredDataTimeSeries:v3",
                "ebIX_DK_MeteredDataTimeSeries-2.xsd",
                "DK_MeteredDataTimeSeries");
            var requestChangeOfSupplierDefinition = CreateDefinition(
                "DK_RequestChangeOfSupplier",
                "un:unece:260:data:EEM-DK_RequestChangeOfSupplier:v3",
                "ebIX_DK_RequestChangeOfSupplier-2.xsd",
                "DK_RequestChangeOfSupplier");
            return CreateDefinition(
                "SendMessageRequest",
                "urn:www:datahub:dk:b2b:v01",
                "DataHUBB2B-manual.xsd",
                "DataHub",
                new List<SchemaDefinition> { meteredDataTimeSeriesDefinition, requestChangeOfSupplierDefinition });
        }

        private static SchemaDefinition CreateDefinition(string rootElement, string targetNamespace, string xsdFile, string schemaSetFolder, IEnumerable<SchemaDefinition> allowedSubSchema)
            => new SchemaDefinition(
                rootElement,
                targetNamespace,
                xsdFile,
                new WsdlEmbeddedResourceLocatorStrategy(schemaSetFolder, xsdFile),
                allowedSubSchema);

        private static SchemaDefinition CreateDefinition(string rootElement, string targetNamespace, string xsdFile, string folder)
            => new SchemaDefinition(
                rootElement,
                targetNamespace,
                xsdFile,
                new EbixEmbeddedResourceLocatorStrategy("DataHub", folder));
    }
}
