using System.Collections.Generic;
using System.Threading.Tasks;

namespace ValidatorTool.RuleEngines
{
    /// <summary>
    /// This provides an common interface for MeterMessage validation that must
    /// be implemented for each concrete IRulesEngine implementation with the
    /// required logic to trigger that rule engine's execution.
    /// </summary>
    public interface IRuleEngine
    {
        Task<bool> ValidateAsync(MeterMessage message);
        Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages);
    }
}