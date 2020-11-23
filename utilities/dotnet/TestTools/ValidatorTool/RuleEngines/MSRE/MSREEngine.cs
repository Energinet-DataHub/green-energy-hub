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

using System.Collections.Generic;
using System.Threading.Tasks;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;

namespace ValidatorTool.RuleEngines.MSRE
{
    /// <summary>
    /// Triggers validation using the Microsoft RulesEngine
    /// </summary>
    public class MSREEngine : IRuleEngine
    {
        private readonly IWorkflowRulesStorage _storage;
        private RulesEngine.RulesEngine _rulesEng = null;

        public MSREEngine(IWorkflowRulesStorage storage)
        {
            _storage = storage;
        }

        public async Task<bool> ValidateAsync(MeterMessage message)
        {
            if (_rulesEng == null)
            {
                var workflowRules = await _storage.GetRulesAsync();
                _rulesEng = new RulesEngine.RulesEngine(workflowRules.ToArray(), null);
            }

            var resultList = _rulesEng.ExecuteRule("Basic", message);
            var isValid = true;

            resultList.OnFail(() =>
            {
                isValid = false;
            });

            return isValid;
        }

        public Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages)
        {
            // Not implemented yet.
            return Task.FromResult(true);
        }
    }
}
