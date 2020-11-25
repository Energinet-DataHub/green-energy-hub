// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RulesEngine.Models;
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

            var jsonString = "[{ \"WorkflowName\": \"Basic\", \"Rules\": [{ \"RuleName\": \"Non-NegativeID\", \"SuccessEvent\": \"0\", \"ErrorMessage\": \"One of the IDs was negative.\", \"ErrorType\": \"Error\", \"RuleExpressionType\": \"LambdaExpression\", \"Expression\": \"customerId >= 0 AND meterId >= 0\" }] }]";
            var workflowRules = JsonConvert.DeserializeObject<List<WorkflowRules>>(jsonString);

            _mockBlobWorkflowRulesStorage.Setup(storage => storage.GetRulesAsync()).ReturnsAsync(workflowRules);

            _engine = new MSREEngine(_mockBlobWorkflowRulesStorage.Object);
        }

        [Test]
        public async Task EvaluateRules_NegativeConsumerId_ReturnsFalse()
        {
            var meterMessage = new MeterMessage(1, 1, DateTime.UtcNow, -1);
            var result = await _engine.ValidateAsync(meterMessage).ConfigureAwait(false);
            Assert.IsFalse(result, $"consumerId in {meterMessage} should not be negative");
        }

        [Test]
        public async Task EvaluateRules_NegativeMeterId_ReturnsFalse()
        {
            var meterMessage = new MeterMessage(1, -1, DateTime.UtcNow, 1);
            var result = await _engine.ValidateAsync(meterMessage).ConfigureAwait(false);
            Assert.IsFalse(result, $"meterId in {meterMessage} should not be negative");
        }

        [Test]
        public async Task EvaluateRules_PositiveIds_ReturnsTrue()
        {
            var meterMessage = new MeterMessage(1, 1, DateTime.UtcNow, 1);
            var result = await _engine.ValidateAsync(meterMessage).ConfigureAwait(false);
            Assert.IsTrue(result);
        }
    }
}
