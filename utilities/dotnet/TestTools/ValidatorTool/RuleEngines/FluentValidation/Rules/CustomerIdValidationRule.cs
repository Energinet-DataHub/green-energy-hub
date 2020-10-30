using FluentValidation;

namespace ValidatorTool.RuleEngines.FluentValidation.Rules
{
    public class CustomerIdValidationRule : AbstractValidator<MeterMessage>
    {
        public CustomerIdValidationRule()
        {
            RuleFor(c => c.CustomerId)
                .LessThan(5)
                .WithName("VR.207-01")
                .WithMessage("Rule no.: {PropertyName} failed with: bla bla");
        }
    }
}