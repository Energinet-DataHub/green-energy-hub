using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace ValidatorTool.RuleEngines.NRules.Rules
{
    /// <summary>
    /// Verify if a meter value is non-negative. Note that the non-negative
    /// verification is part of the action body, not the match condition.
    ///
    /// Moving the verification to match condition would not result in a
    /// RuleResult upon validation rule failure.
    /// </summary>
    [Repeatability(RuleRepeatability.NonRepeatable)]
    public class NonNegativeMeterValueRule : Rule
    {
        public override void Define()
        {
            // ValidationResult result = null;
            MeterMessage message = null;

            When()
                .Match<MeterMessage>(() => message);
            Then()
                .Yield(_ => DoValidation(message));
        }

        private RuleResult DoValidation(MeterMessage message)
        {
            if (message.MeterValue < 0)
            {
                return new RuleResult(GetType().Name, message.TransactionId, false, "Meter value was negative");
            }

            return new RuleResult(GetType().Name, message.TransactionId, true);
        }
    }
}
