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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Energinet.DataHub.SoapAdapter.Application.Exceptions
{
    [Serializable]
    public sealed class UnknownConverterException : Exception
    {
        private const string Unknown = "Unknown";

        public UnknownConverterException()
        {
            DocumentType = Unknown;
        }

        public UnknownConverterException(string documentType)
            : base(documentType)
        {
            DocumentType = documentType;
        }

        public UnknownConverterException(string documentType, Exception innerException)
            : base(documentType, innerException)
        {
            DocumentType = documentType;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private UnknownConverterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DocumentType = info.GetString("DocumentType") !;
        }

        public string DocumentType { get; }
    }
}
