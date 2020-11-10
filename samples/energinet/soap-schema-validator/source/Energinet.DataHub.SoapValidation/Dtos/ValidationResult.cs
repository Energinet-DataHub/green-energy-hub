using System.Collections.Generic;

namespace Energinet.DataHub.SoapValidation.Dtos
{
    public class ValidationResult
    {
        public ValidationResult(bool isSuccess, RejectionReason reason, List<ValidationProblem> problems)
            : this(isSuccess, string.Empty, reason, problems)
        {
        }

        public ValidationResult(bool isSuccess, string senderEnergyPartyIdentification, RejectionReason reason, List<ValidationProblem> problems)
        {
            IsSuccess = isSuccess;
            SenderEnergyPartyIdentification = senderEnergyPartyIdentification;
            RejectionReason = reason;
            ValidationProblems = problems;
        }

        public bool IsSuccess { get; }

        public string SenderEnergyPartyIdentification { get; }

        public RejectionReason RejectionReason { get; }

        public List<ValidationProblem> ValidationProblems { get; }
    }
}
