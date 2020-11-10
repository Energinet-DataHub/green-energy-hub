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
            return CreateDefinition(
                "SendMessageRequest",
                "urn:www:datahub:dk:b2b:v01",
                "DataHUBB2B-manual.xsd",
                "DataHub",
                new List<SchemaDefinition> { meteredDataTimeSeriesDefinition });
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
