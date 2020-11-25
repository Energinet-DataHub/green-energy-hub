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
using CsvHelper.Configuration.Attributes;

namespace TimeSeriesGenerator.Domain
{
    public class SampleData
    {
        public string GridArea { get; set; }

        [Name("Type_Of_MP")]
        public string TypeOfMp { get; set; }

        [Name("Physical_Status")]
        public string PhysicalStatus { get; set; }

        [Name("Settlement_Method")]
        public string SettlementMethod { get; set; }

        public string Count { get; set; }
    }
}
