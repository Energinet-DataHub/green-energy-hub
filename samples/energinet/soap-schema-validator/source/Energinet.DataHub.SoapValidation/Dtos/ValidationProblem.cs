using System.Xml.Schema;

namespace Energinet.DataHub.SoapValidation.Dtos
{
    public class ValidationProblem
    {
        public ValidationProblem(string message, XmlSeverityType severity, int lineNumber, int linePosition)
        {
            Message = message;
            Severity = severity;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        public string Message { get; }

        public XmlSeverityType Severity { get; }

        public int LineNumber { get; }

        public int LinePosition { get; }
    }
}
