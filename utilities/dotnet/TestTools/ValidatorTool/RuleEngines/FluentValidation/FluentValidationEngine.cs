using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ValidatorTool.RuleEngines.FluentValidation.Rules;

namespace ValidatorTool.RuleEngines.FluentValidation
{
    public class FluentValidationEngine : IRuleEngine
    {
        private readonly AbstractValidator<MeterMessage> _validator;

        public FluentValidationEngine()
        {
            _validator = new MeterMessageValidator();
        }

        public async Task<bool> ValidateAsync(MeterMessage message)
        {
            var result = await _validator.ValidateAsync(message);

            return result.IsValid;
        }

        public async Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages)
        {
            var results = messages.Select(m => _validator.ValidateAsync(m)).ToArray();
            await Task.WhenAll(results);

            return results.All(r => r.Result.IsValid);
        }
    }
}
