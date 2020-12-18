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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Postoffice
{
    public class EnerginetDoc
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Vendor { get; set; }

        public double MeteringPointValue { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public static EnerginetDoc FromString(string input)
        {
            return JsonConvert.DeserializeObject<EnerginetDoc>(input);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
