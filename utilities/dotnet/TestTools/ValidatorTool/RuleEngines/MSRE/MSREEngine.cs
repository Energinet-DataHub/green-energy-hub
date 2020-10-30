using RulesEngine.Models;
using static RulesEngine.Extensions.ListofRuleResultTreeExtension;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ValidatorTool.RuleEngines.MSRE
{
    /// <summary>
    /// Triggers validation using the Microsoft RulesEngine
    /// </summary>
    public class MSREEngine : IRuleEngine
    {
        private readonly IWorkflowRulesStorage storage;
        private RulesEngine.RulesEngine _rulesEng = null;
        public MSREEngine(IWorkflowRulesStorage storage){
            this.storage = storage;
        }
        public async Task<bool> ValidateAsync(MeterMessage message)
        {
            if (_rulesEng == null)
            {
                var workflowRules = await storage.GetRulesAsync();
                _rulesEng = new RulesEngine.RulesEngine(workflowRules.ToArray(), null);
            }

            List<RuleResultTree> resultList = _rulesEng.ExecuteRule("Basic", message);
            bool isValid = true;

            resultList.OnFail(() =>
            {
                isValid = false;
            });

            return isValid;
        }

        public async Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages) {
            throw new NotImplementedException();
        }
    }
}