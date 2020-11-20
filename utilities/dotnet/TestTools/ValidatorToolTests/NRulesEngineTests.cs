using System;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.NRules;
using Xunit;

namespace NRulesMeterRulesTests
{
    /// <summary>
    /// Tests of the NRules implementation, using the xUnit framework
    /// </summary>
    public class NRulesEngineTests
    {
        private readonly IRuleEngine _ruleEngine;

        public NRulesEngineTests()
        {
            _ruleEngine = new NRulesEngine();
        }

        [Fact]
        public async void NonNegativeValueAndValidCustomerId_ReturnsTrue()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 1);
            Assert.True(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NonNegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NegativeValueAndValidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 3);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }

        [Fact]
        public async void NegativeValueAndInvalidCustomerId_ReturnsFalse()
        {
            var m = new MeterMessage(-1, 1, DateTime.UtcNow.ToUniversalTime(), 6);
            Assert.False(await _ruleEngine.ValidateAsync(m));
        }
    }
}
