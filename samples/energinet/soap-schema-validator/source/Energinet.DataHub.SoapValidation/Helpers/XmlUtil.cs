using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapValidation.Dtos;

namespace Energinet.DataHub.SoapValidation.Helpers
{
    internal static class XmlUtil
    {
        /// <summary>
        /// Retrieve a <see cref="DocumentDefinition"/> from a xml-stream
        /// </summary>
        /// <param name="xmlStream">xml-stream to search</param>
        /// <returns><see cref="DocumentDefinition"/> for the xml-stream</returns>
        /// <exception cref="XmlException">If no definition if found due to invalid xml</exception>
        public static async Task<DocumentDefinition> GetDocumentIdentificationAsync(Stream xmlStream)
        {
            var position = xmlStream.Position;

            var settings = new XmlReaderSettings
            {
                Async = true,
            };

            try
            {
                using var reader = XmlReader.Create(xmlStream, settings);

                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        return new DocumentDefinition(reader.LocalName, reader.NamespaceURI);
                    }
                }

                throw new XmlException("Unable to locate root element and/or namespace");
            }
            finally
            {
                xmlStream.Position = position;
            }
        }
    }
}
