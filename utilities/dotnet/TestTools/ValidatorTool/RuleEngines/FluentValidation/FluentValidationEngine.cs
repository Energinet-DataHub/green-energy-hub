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
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ValidatorTool.RuleEngines.FluentValidation.Rules;

namespace ValidatorTool.RuleEngines.FluentValidation
{
    public class FluentValidationEngine : IRuleEngine
    {
        private readonly AbstractValidator<MeterMessage> _validator;

        public FluentValidationEngine()
        {
            _validator = new MeterMessageValidator();
        }

        public async Task<bool> ValidateAsync(MeterMessage message)
        {
            var result = await _validator.ValidateAsync(message);

            return result.IsValid;
        }

        public async Task<bool> ValidateBatchAsync(IEnumerable<MeterMessage> messages)
        {
            var results = messages.Select(m => _validator.ValidateAsync(m)).ToArray();
            await Task.WhenAll(results);

            return results.All(r => r.Result.IsValid);
        }
    }
}
