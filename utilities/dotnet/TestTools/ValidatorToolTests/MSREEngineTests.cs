using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ValidatorTool;
using ValidatorTool.RuleEngines;
using ValidatorTool.RuleEngines.MSRE;

namespace ValidatorTool.Tests
{
    /// <summary>
    /// Tests of the MSRE implementation, using the NUnit framework
    /// </summary>
    [TestFixture]
    public class REEngineTests
    {
        private IRuleEngine _engine;
        private Mock<IWorkflowRulesStorage> _mockBlobWorkflowRulesStorage;

        [SetUp]
        public void Setup()
        {
            _mockBlobWorkflowRulesStorage = new Mock<IWorkflowRulesStorage>();

            var jsonString ="[{ \"WorkflowName\": \"Basic\", \"Rules\": [{ \"RuleName\": \"Non-NegativeID\", \"SuccessEvent\": \"0\", \"ErrorMessage\": \"One of the IDs was negative.\", \"ErrorType\": \"Error\", \"RuleExpressionType\": \"LambdaExpression\", \"Expression\": \"customerId >= 0 AND meterId >= 0\" }] }]";
            var workflowRules = JsonConvert.DeserializeObject<List<WorkflowRules>>(jsonString);

            _mockBlobWorkflowRulesStorage.Setup(storage => storage.GetRulesAsync()).ReturnsAsync(workflowRules);

            _engine = new MSREEngine(_mockBlobWorkflowRulesStorage.Object);
        }

        [Test]
        public async Task EvaluateRules_NegativeConsumerId_ReturnsFalse()
        {
            var meterMessage = new MeterMessage(1, 1, DateTime.UtcNow, -1);
            var result = await _engine.ValidateAsync(meterMessage);
            Assert.IsFalse(result, $"consumerId in {meterMessage} should not be negative");
        }

        [Test]
        public async Task EvaluateRules_NegativeMeterId_ReturnsFalse()
        {
            var meterMessage = new MeterMessage(1, -1, DateTime.UtcNow, 1);
            var result = await _engine.ValidateAsync(meterMessage);
            Assert.IsFalse(result, $"meterId in {meterMessage} should not be negative");
        }

        [Test]
        public async Task EvaluateRules_PositiveIds_ReturnsTrue()
        {
            var meterMessage = new MeterMessage(1, 1, DateTime.UtcNow, 1);
            var result = await _engine.ValidateAsync(meterMessage);
            Assert.IsTrue(result);
        }
    }
}