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

namespace Energinet.DataHub.SoapAdapter.Tests.Fixtures
{
    public class DocumentTypesFixture : TheoryData<string>
    {
        public DocumentTypesFixture()
        {
            Add("RequestChangeOfSupplier");
            Add("RequestCancelChangeOfSupplier");
            Add("RequestEndOfSupply");
            Add("RequestMPCharacteristics");
            Add("NotifyMPCharacteristics");
            Add("Acknowledgement");
            Add("MeteredDataProfiled");
            Add("MeteredDataTimeSeries");
            Add("RequestMeteredDataAggregated");
            Add("RequestMeteredDataValidated");
            Add("ConfirmChangeOfSupplier");
            Add("RejectChangeOfSupplier");
            Add("ConfirmCancelChangeOfSupplier");
            Add("RejectCancelChangeOfSupplier");
            Add("NotifyChangeOfSupplier");
            Add("ConfirmEndOfSupply");
            Add("RejectEndOfSupply");
            Add("LoadProfileShareMeteredDataTimeSeries");
            Add("AggregatedMeteredDataTimeSeries");
            Add("RejectRequestMeteredData");
        }
    }
}
