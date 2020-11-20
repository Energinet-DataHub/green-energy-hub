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
