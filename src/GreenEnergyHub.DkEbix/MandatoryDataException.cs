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

namespace GreenEnergyHub.DkEbix
{
    /// <summary>
    /// Mandatory data is not provided
    /// </summary>
    public class MandatoryDataException : Exception
    {
        /// <summary>
        /// Create an exception for missing data
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="field">field that is missing the data</param>
        public MandatoryDataException(string message, string field)
            : base(message)
        {
            Field = field;
        }

        public string Field { get; }

        public override string ToString()
        {
            return $"Field: {Field} - {Message}";
        }
    }
}
