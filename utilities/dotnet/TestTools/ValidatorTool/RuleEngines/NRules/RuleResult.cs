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

namespace ValidatorTool.RuleEngines.NRules
{
    /// <summary>
    /// Used to track the execution result of each validation rule. Instances of
    /// this class are inserted as linked facts, enabling us to query the NRules
    /// session for a collection of validation results after inserting the
    /// message. RuleResult is also a key part of enabling inter-rule
    /// dependencies (forward chaining).
    /// </summary>
    public class RuleResult
    {
        public RuleResult(string rule, string transactionId, bool isSuccessful, string message = null)
        {
            RuleName = rule;
            TransactionId = transactionId;
            IsSuccessful = isSuccessful;
            Message = message;
        }

        public string RuleName { get; }

        public string TransactionId { get; }

        public bool IsSuccessful { get; }

        public string Message { get; }
    }
}
