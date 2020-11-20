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
