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
