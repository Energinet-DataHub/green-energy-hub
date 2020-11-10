using System.IO;
using System.Threading.Tasks;
using Energinet.DataHub.SoapValidation.Dtos;

namespace Energinet.DataHub.SoapValidation
{
    /// <summary>
    /// Interface used to validate that streams are indeed valid XML and follow the rules defined by the schemas
    /// </summary>
    public interface IXmlSchemaValidator
    {
        /// <summary>
        /// Validates that the stream is valid XML in all <see cref="SchemaCollections"/>. Sub definitions are not searched
        /// </summary>
        /// <param name="stream">The stream to validate. The position in the stream is restored after validation</param>
        /// <returns><see cref="ValidationResult"/> with the result</returns>
        Task<ValidationResult> ValidateStreamAsync(Stream stream);

        /// <summary>
        /// Validates that the stream is valid XML, optional included sub definitions
        /// </summary>
        /// <param name="stream">The stream to validate. The position in the stream is restored after validation</param>
        /// <param name="traverseSubDefinitions">true - if sub definitions should be searched</param>
        /// <returns><see cref="ValidationResult"/> with the result</returns>
        Task<ValidationResult> ValidateStreamAsync(Stream stream, bool traverseSubDefinitions);
    }
}
