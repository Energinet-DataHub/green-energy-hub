using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Json.Schema;

namespace GreenEnergyHub.JSONSchemaValidator.Validate
{
    /// <summary>
    /// Validation service
    /// </summary>
    public class ValidateService
    {
        private readonly ValidationOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateService"/> class.
        /// </summary>
        /// <param name="options">Validation settings</param>
        public ValidateService(ValidationOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Validates the document according to the given schema type
        /// </summary>
        /// <param name="type">The type of schema that's about to be validated</param>
        /// <param name="json">The document to be validated</param>
        public ValidationResults ValidateDocument(SchemaType type, JsonDocument json)
        {
            var schema = JsonSchema.FromFile($"schemas/{type}.schema.json");

            return schema.Validate(json.RootElement, _options);
        }
    }
}
