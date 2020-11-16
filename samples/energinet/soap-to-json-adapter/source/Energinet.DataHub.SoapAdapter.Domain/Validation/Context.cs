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

namespace Energinet.DataHub.SoapAdapter.Domain.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a validation context for a generic SOAP RSM request
    /// </summary>
    public class Context
    {
        public Context()
        {
            RsmHeader = new RsmHeader();
        }

        /// <summary>
        /// Rsm document header
        /// </summary>
        public RsmHeader RsmHeader
        {
            get;
        }

        /// <summary>
        /// Content length of the request
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// Message reference for the request
        /// </summary>
        public string MessageReference { get; set; } = string.Empty;

        /// <summary>
        /// RSM document type
        /// </summary>
        public DocumentType RsmDocumentType { get; set; } = DocumentType.Default;

        /// <summary>
        /// Transaction ids for all payloads
        /// </summary>
        public List<string> TransactionIds { get; set; } = new List<string>();
    }
}
