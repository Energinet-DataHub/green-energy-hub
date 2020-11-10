namespace Energinet.DataHub.SoapValidation.Dtos
{
    public enum RejectionReason : int
    {
        None = 0,
        SchemaNotUsed = 1,
        DoesNotRespectSchema = 2,
        SchemasUnavailable = 3,
        InvalidXml = 4,
        UnableToValidate = 5,
    }
}
