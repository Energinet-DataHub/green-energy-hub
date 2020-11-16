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
namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RulesEngine
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
        /// <summary>
        /// Creates an instance of the RuleResult POCO.
        /// </summary>
        /// <param name="rule">The name of the rule.</param>
        /// <param name="transactionId">The transaction id of the message which
        /// triggered this rule result.</param>
        /// <param name="isSuccessful">Whether the message associated with the
        /// given transactionId successfully passed the given rule.</param>
        /// <param name="details">Any other details.</param>
        public RuleResult(string rule, string transactionId, bool isSuccessful, string details = "")
        {
            RuleName = rule;
            TransactionId = transactionId;
            IsSuccessful = isSuccessful;
            Details = details;
        }

        /// <summary>
        /// The name of the rule tied to this result.
        /// </summary>
        public string RuleName { get; }

        /// <summary>
        /// The id of the transaction tied to this result.
        /// </summary>
        public string TransactionId { get; }

        /// <summary>
        /// The success status tied to this result.
        /// </summary>
        public bool IsSuccessful { get; }

        /// <summary>
        /// Any other details tied to this result.
        /// </summary>
        public string Details { get; }
    }
}
