using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace ValidatorTool.RuleEngines.NRules.Rules
{
    /// <summary>
    /// Verify if a customer ID is "valid" (i.e., below 5 for a test case here).
    ///
    /// This rule implements forward chaining, and only executes once its
    /// dependent rule (NonNegativeMeterValueRule) has executed.
    /// </summary>
    [Repeatability(RuleRepeatability.NonRepeatable)]
    public class CustomerIdValidationRule : Rule
    {
        public override void Define()
        {
            // ValidationResult result = null;
            MeterMessage message = null;
            RuleResult result = null;

            When()
                .Match<MeterMessage>(() => message)
                .Match<RuleResult>(() => result, r => r.RuleName == "NonNegativeMeterValueRule" && r.TransactionId == message.TransactionId && r.IsSuccessful);
            Then()
                .Yield(_ => DoValidation(message));
        }

        private RuleResult DoValidation(MeterMessage message)
        {
            if (IsValidCustomer(message.CustomerId))
            {
                return new RuleResult(GetType().Name, message.TransactionId, true);
            }
            else
            {
                return new RuleResult(GetType().Name, message.TransactionId, false, "Customer ID was invalid");
            }
        }

        private bool IsValidCustomer(int customerId)
        {
            // Console.WriteLine("Mock fetching from database..."); // TODO: eventually we'll want this to talk to SQL
            return customerId < 5; // this check doesn't make sense logically, we just want it to fail half the values
        }
    }
}
