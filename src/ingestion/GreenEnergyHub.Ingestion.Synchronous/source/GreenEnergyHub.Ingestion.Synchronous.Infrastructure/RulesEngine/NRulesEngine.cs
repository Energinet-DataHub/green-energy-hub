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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GreenEnergyHub.Ingestion.Synchronous.Application;
using NRules;
using NRules.Fluent;

namespace GreenEnergyHub.Ingestion.Synchronous.Infrastructure.RulesEngine
{
    /// <summary>
    /// An implementation of the IRuleEngine backed by the NRules library.
    /// </summary>
    public class NRulesEngine<TRequest> : IRuleEngine<TRequest>
        where TRequest : IHubActionRequest
    {
        private readonly ISession _session;

        /// <summary>
        /// Creates an NRules session from a given ruleset.
        /// </summary>
        /// <param name="ruleSet">The rules to create the NRules session from.
        /// </param>
        public NRulesEngine(IHubRuleSet<TRequest> ruleSet)
        {
            // Load rules
            var repository = new RuleRepository();
            repository.Load(x => x.From(ruleSet.Rules.Select(_ => _.MakeGenericType(typeof(TRequest)))));

            // Compile rules
            var factory = repository.Compile();

            // Create a working session
            _session = factory.CreateSession();
        }

        /// <summary>
        /// Validates a message given this NRuleEngine's associated ruleset.
        /// </summary>
        /// <param name="request">The message to validate.</param>
        /// <returns>True if the message is valid according to this instance's
        /// provided ruleset.</returns>
        public Task<bool> ValidateAsync(TRequest request)
        {
            _session.Insert(request);

            // Start match/resolve/act cycle
            _session.Fire();

            // TODO: This should be moved to a _logger.LogDebug()
            Console.WriteLine($"Message {JsonSerializer.Serialize(request)}");
            var results = _session.Query<RuleResult>();
            foreach (RuleResult result in results)
            {
                if (result.IsSuccessful)
                {
                    Console.WriteLine($"\tRule {result.RuleName} was validated.");
                }
                else
                {
                    Console.WriteLine($"\tRule {result.RuleName} failed to validate: {result.Details}");
                }
            }

            var valid = results.All(r => r.IsSuccessful); // Must get result before retracting message because will remove linked facts

            _session.Retract(request);
            return Task.FromResult(valid);
        }
    }
}
