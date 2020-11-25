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
    using System.Collections.Generic;

    public class DocumentType
    {
        private static readonly HashSet<string> _validDocumentTypes = new HashSet<string>(new[]
        {
            DocumentTypes.RequestChangeOfSupplier,
            DocumentTypes.RequestCancelChangeOfSupplier,
            DocumentTypes.RequestEndOfSupply,
            DocumentTypes.RequestMpCharacteristics,
            DocumentTypes.NotifyMpCharacteristics,
            DocumentTypes.Acknowledgement,
            DocumentTypes.MeteredDataProfiled,
            DocumentTypes.MeteredDataTimeSeries,
            DocumentTypes.RequestMeteredDataAggregated,
            DocumentTypes.RequestMeteredDataValidated,
            DocumentTypes.ConfirmChangeOfSupplier,
            DocumentTypes.RejectChangeOfSupplier,
            DocumentTypes.ConfirmCancelChangeOfSupplier,
            DocumentTypes.RejectCancelChangeOfSupplier,
            DocumentTypes.NotifyChangeOfSupplier,
            DocumentTypes.ConfirmEndOfSupply,
            DocumentTypes.RejectEndOfSupply,
            DocumentTypes.LoadProfileShareMeteredDataTimeSeries,
            DocumentTypes.AggregatedMeteredDataTimeSeries,
            DocumentTypes.RejectRequestMeteredData,
        });

        public DocumentType(string documentType)
        {
            Value = documentType;
        }

        public static DocumentType Default => new DocumentType(string.Empty);

        public string Value { get; }

        public static bool IsValid(string documentType)
        {
            return _validDocumentTypes.Contains(documentType);
        }

        public bool IsValid()
        {
            return IsValid(Value);
        }
    }
}
