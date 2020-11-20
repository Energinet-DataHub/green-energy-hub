using System;
using System.Threading.Tasks;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using Xunit;

namespace ValidatorToolTests
{
    public class FluentValidationTests
    {
        private readonly IRuleEngine _ruleEngine;

        public FluentValidationTests()
        {
            _ruleEngine = new FluentValidationEngine();
        }

        [Fact]
        public async Task NonNegativeValueAndValidCustomerId_ReturnsTrue()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 1);
            Assert.True(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NonNegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NegativeValueAndValidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 3);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async Task NegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }
    }
}
