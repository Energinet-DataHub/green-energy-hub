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
using Energinet.DataHub.MarketData.Domain.SeedWork;

namespace Energinet.DataHub.MarketData.Domain.MarketEvaluationPoints.Rules
{
    internal class MarketEvaluationPointIdMustBeGsrnCompliantRule : IBusinessRule
    {
        private const int RequiredIdLength = 18;
        private readonly string _gsrnValue;

        public MarketEvaluationPointIdMustBeGsrnCompliantRule(string gsrnValue)
        {
            _gsrnValue = gsrnValue;
        }

        public bool IsBroken => !IsValidGsrnNumber();

        public string Message => $"Invalid GSRN number.";

        private static int Parse(string input)
        {
            return int.Parse(input, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        private bool IsValidGsrnNumber()
        {
            return LengthIsValid() && StartDigitsAreValid() && CheckSumIsValid();
        }

        private bool LengthIsValid()
        {
            return _gsrnValue.Length == RequiredIdLength;
        }

        private bool StartDigitsAreValid()
        {
            int startDigits = Parse(_gsrnValue.Substring(0, 2));
            return startDigits == 57;
        }

        private bool CheckSumIsValid()
        {
            int definedChecksumDigit = Parse(_gsrnValue.Substring(_gsrnValue.Length - 1));
            int calculatedChecksum = CalculateChecksum();
            return calculatedChecksum == definedChecksumDigit;
        }

        private int CalculateChecksum()
        {
            int sum = 0;
            bool positionIsOdd = true;
            for (int currentPosition = 1; currentPosition < RequiredIdLength; currentPosition++)
            {
                int currentValueAtPosition = Parse(_gsrnValue.Substring(currentPosition - 1, 1));
                if (positionIsOdd)
                {
                    sum = sum + (currentValueAtPosition * 3);
                }
                else
                {
                    sum = sum + (currentValueAtPosition * 1);
                }

                positionIsOdd = !positionIsOdd;
            }

            int equalOrHigherMultipleOf = (int)(Math.Ceiling(sum / 10.0) * 10);

            return equalOrHigherMultipleOf - sum;
        }
    }
}
