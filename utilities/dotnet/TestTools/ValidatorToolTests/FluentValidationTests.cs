using System;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.FluentValidation;
using Xunit;

namespace ValidatorToolTests
{
    public class FluentValidationTests
    {
        private readonly IRuleEngine ruleEngine;

        public FluentValidationTests() {
            ruleEngine = new FluentValidationEngine();
        }

        [Fact]
        public async void NonNegativeValueAndValidCustomerId_ReturnsTrue()
        {
            MeterMessage m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 1);
            Assert.True(await ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NonNegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            MeterMessage m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NegativeValueAndValidCustomerId_ReturnsFalse()
        {
            MeterMessage m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 3);
            Assert.False(await ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            MeterMessage m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await ruleEngine.ValidateAsync(m));
        }
    }
}