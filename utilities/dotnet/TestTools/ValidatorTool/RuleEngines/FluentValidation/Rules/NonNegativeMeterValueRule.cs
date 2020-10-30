using FluentValidation;

namespace ValidatorTool.RuleEngines.FluentValidation.Rules
{
    public class NonNegativeMeterValueRule : AbstractValidator<MeterMessage>
    {
        public NonNegativeMeterValueRule()
        {
            RuleFor(m => m.MeterValue)
                .GreaterThan(0);
        }
    }
}