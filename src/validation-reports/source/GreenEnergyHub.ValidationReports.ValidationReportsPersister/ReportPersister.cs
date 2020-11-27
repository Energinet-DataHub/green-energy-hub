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

using System.Text.Json;
using GreenEnergyHub.ValidationReports.Domain.Models;
using GreenEnergyHub.ValidationReports.Infrastructure.Models;
using Microsoft.Azure.WebJobs;

namespace GreenEnergyHub.ValidationReports.ValidationReportsPersister
{
    public class ReportPersister
    {
        private readonly JsonSerializerOptions _options;

        public ReportPersister(JsonSerializerOptions options)
        {
            _options = options;
        }

        [FunctionName("ReportPersister")]
        [return: Table("ValidationReportTable", Connection = "ValidationReportStorageConnectionString")]
        public ValidationResultContainerBlob Run(
            [EventHubTrigger("evh-validation-reports-sandbox", Connection = "ValidationReportEventHubConnectionString")]
            string queueItem)
        {
            var report = JsonSerializer.Deserialize<ValidationResultContainer>(queueItem, _options);

            return ValidationResultContainerBlob.FromReport(report);
        }
    }
}
