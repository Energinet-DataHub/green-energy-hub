using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NRules;
using NRules.Fluent;
using ValidatorTool.RuleEngines.NRules.Rules;

namespace ValidatorTool.RuleEngines.NRules
{
    /// <summary>
    /// Triggers validation using the NRules project
    /// </summary>
    public class NRulesEngine : IRuleEngine
    {
        private readonly ISession _session;

        public NRulesEngine()
        {
            // Load rules
            var repository = new RuleRepository();
            repository.Load(x => x.From(typeof(NonNegativeMeterValueRule).Assembly));

            // Compile rules
            var factory = repository.Compile();

            // Create a working session
            _session = factory.CreateSession();
        }

        public Task<bool> ValidateAsync(MeterMessage message)
        {
            _session.Insert(message);

            // Start match/resolve/act cycle
            _session.Fire();

            // TODO: This should be moved to a _logger.LogDebug()
            var results = _session.Query<RuleResult>();
            var valid = results.All(r => r.IsSuccessful); // Must get result before retracting message because will remove linked facts

            _session.Retract(message);
            return Task.FromResult(valid);
        }

        public async Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages)
        {
            _session.InsertAll(messages);

            // Start match/resolve/act cycle
            _session.Fire();

            // TODO: This should be moved to a _logger.LogDebug()
            var results = _session.Query<RuleResult>();
            var valid = results.All(r => r.IsSuccessful); // Must get result before retracting message because will remove linked facts

            _session.RetractAll(messages);
            return valid;
        }
    }
}
