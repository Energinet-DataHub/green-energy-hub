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

namespace Energinet.DataHub.SoapAdapter.Domain
{
    public static class DocumentTypes
    {
        public const string RequestChangeOfSupplier = "RequestChangeOfSupplier";
        public const string RequestCancelChangeOfSupplier = "RequestCancelChangeOfSupplier";
        public const string RequestEndOfSupply = "RequestEndOfSupply";
        public const string RequestMpCharacteristics = "RequestMPCharacteristics";
        public const string NotifyMpCharacteristics = "NotifyMPCharacteristics";
        public const string Acknowledgement = "Acknowledgement";
        public const string MeteredDataProfiled = "MeteredDataProfiled";
        public const string MeteredDataTimeSeries = "MeteredDataTimeSeries";
        public const string RequestMeteredDataAggregated = "RequestMeteredDataAggregated";
        public const string RequestMeteredDataValidated = "RequestMeteredDataValidated";
        public const string ConfirmChangeOfSupplier = "ConfirmChangeOfSupplier";
        public const string RejectChangeOfSupplier = "RejectChangeOfSupplier";
        public const string ConfirmCancelChangeOfSupplier = "ConfirmCancelChangeOfSupplier";
        public const string RejectCancelChangeOfSupplier = "RejectCancelChangeOfSupplier";
        public const string NotifyChangeOfSupplier = "NotifyChangeOfSupplier";
        public const string ConfirmEndOfSupply = "ConfirmEndOfSupply";
        public const string RejectEndOfSupply = "RejectEndOfSupply";
        public const string LoadProfileShareMeteredDataTimeSeries = "LoadProfileShareMeteredDataTimeSeries";
        public const string AggregatedMeteredDataTimeSeries = "AggregatedMeteredDataTimeSeries";
        public const string RejectRequestMeteredData = "RejectRequestMeteredData";
    }
}
