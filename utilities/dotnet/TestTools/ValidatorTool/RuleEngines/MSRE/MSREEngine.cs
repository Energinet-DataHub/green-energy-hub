using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Models;
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
