using FluentValidation;

namespace ValidatorTool.RuleEngines.FluentValidation.Rules
{
    public class MeterMessageValidator : AbstractValidator<MeterMessage>
    {
        public MeterMessageValidator()
        {
            Include(new CustomerIdValidationRule());
            Include(new NonNegativeMeterValueRule());
        }
    }
}