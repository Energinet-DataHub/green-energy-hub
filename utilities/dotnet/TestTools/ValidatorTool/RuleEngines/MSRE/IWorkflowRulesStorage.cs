using System.Collections.Generic;
using System.Threading.Tasks;
using RulesEngine.Models;

public interface IWorkflowRulesStorage
{
    public Task<List<WorkflowRules>> GetRulesAsync();
}
