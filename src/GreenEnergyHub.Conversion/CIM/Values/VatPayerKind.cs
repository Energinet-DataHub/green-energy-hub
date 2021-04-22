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

namespace GreenEnergyHub.Conversion.CIM.Values
{
    public sealed class VatPayerKind : ValueType<string>
    {
        private const string D01 = "D01";
        private const string D02 = "D02";

        private VatPayerKind(string value)
            : base(value)
        {
        }

        public static VatPayerKind PayedByIssuer { get; } = new (D01);

        public static VatPayerKind PayedByRecipient { get; } = new (D02);

        public static bool TryParse(string vatPayerCode, out VatPayerKind? vatPayerKind)
        {
            vatPayerKind = null;
            if (vatPayerCode.Equals(D01, StringComparison.InvariantCulture))
            {
                vatPayerKind = PayedByIssuer;
            }
            else if (vatPayerCode.Equals(D02, StringComparison.InvariantCulture))
            {
                vatPayerKind = PayedByRecipient;
            }

            return vatPayerKind != null;
        }

        public static VatPayerKind Parse(string vatPayerCode)
        {
            if (vatPayerCode.Equals(D01, StringComparison.InvariantCulture)) return PayedByIssuer;
            if (vatPayerCode.Equals(D02, StringComparison.InvariantCulture)) return PayedByRecipient;

            throw new ArgumentOutOfRangeException(nameof(vatPayerCode), "Value is not a valid enumeration");
        }
    }
}
