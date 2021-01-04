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

using NodaTime;

namespace Energinet.DataHub.SoapAdapter.Domain.Validation
{
    using System;

    /// <summary>
    /// Rsm document header, which includes HeaderEnergyDocument and ProcessEnergyContext
    /// </summary>
    public sealed class RsmHeader
    {
        /// <summary>
        /// The message reference from the message container
        /// </summary>
        public string? MessageReference { get; set; }

        /// <summary>
        /// Some id
        /// </summary>
        public string? Identification { get; set; }

        /// <summary>
        /// What type the received message is eg. E66.
        /// </summary>
        public string? DocumentType { get; set; }

        /// <summary>
        /// Time of when the time series was created
        /// </summary>
        public Instant? Creation { get; set; }

        /// <summary>
        /// The ID of the sender of the time series message
        /// </summary>
        public string? SenderIdentification { get; set; }

        /// <summary>
        /// The ID of the recipient of the time series
        /// </summary>
        public string? RecipientIdentification { get; set; }

        /// <summary>
        /// This is a business reason code. It informs you about the context the RSM-message is used in.
        /// E.g. a RSM-012 time series with code EnergyBusinessProcess = 'D42' is a time series for a flex metering point.
        /// </summary>
        public string? EnergyBusinessProcess { get; set; }

        /// <summary>
        /// Process Role information eg. MDR.
        /// </summary>
        public string? EnergyBusinessProcessRole { get; set; }

        /// <summary>
        /// Sector area information
        /// </summary>
        public string? EnergyIndustryClassification { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == typeof(RsmHeader) && Equals((RsmHeader)obj);
        }

        public override int GetHashCode()
        {
            var id = Identification + MessageReference; // Needed because HashCode combine only allows 8 params.
            return HashCode.Combine(id, DocumentType, Creation, SenderIdentification, RecipientIdentification, EnergyBusinessProcess, EnergyBusinessProcessRole, EnergyIndustryClassification);
        }

        private bool Equals(RsmHeader other)
        {
            return MessageReference == other.MessageReference && Identification == other.Identification && DocumentType == other.DocumentType && Nullable.Equals(Creation, other.Creation) && SenderIdentification == other.SenderIdentification && RecipientIdentification == other.RecipientIdentification && EnergyBusinessProcess == other.EnergyBusinessProcess && EnergyBusinessProcessRole == other.EnergyBusinessProcessRole && EnergyIndustryClassification == other.EnergyIndustryClassification;
        }
    }
}
