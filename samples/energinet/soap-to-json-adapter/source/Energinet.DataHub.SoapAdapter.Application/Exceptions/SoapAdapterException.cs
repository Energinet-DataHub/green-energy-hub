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
    public class SoapAdapterException : Exception
    {
        private const string Unknown = "Unknown";

        public SoapAdapterException()
        {
            MessageReference = Unknown;
            ErrorMessage = Unknown;
        }

        public SoapAdapterException(string message)
            : base(message)
        {
            MessageReference = Unknown;
            ErrorMessage = message;
        }

        public SoapAdapterException(string message, Exception innerException)
            : base(message, innerException)
        {
            MessageReference = Unknown;
            ErrorMessage = message;
        }

        public SoapAdapterException(string messageReference, string errorMessage)
            : base($"{messageReference}:{errorMessage}")
        {
            MessageReference = messageReference;
            ErrorMessage = errorMessage;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SoapAdapterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            MessageReference = info.GetString("MessageReference") !;
            ErrorMessage = info.GetString("ErrorMessage") !;
        }

        public string MessageReference { get; }

        public string ErrorMessage { get; }
    }
}
