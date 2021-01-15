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
namespace Energinet.DataHub.SoapAdapter.Application.Converters.Fragments.RSM012
{
    /// <summary>
    /// Class representing the bits and pieces of the original ebiX document that we actually
    /// need in the conversion to CIM JSON documents.
    ///
    /// Since the conversion is mostly unstructured, this allows us to pass the various parsed information
    /// around without having to pass a lot of arguments.
    /// </summary>
    internal class RSM012Fragments
    {
        internal string? PayloadEnergyTimeSeriesIdentification { get; set; }

        internal ObservationTimeSeriesPeriod? ObservationTimeSeriesPeriod { get; set; }

        internal IncludedProductCharacteristic? IncludedProductCharacteristic { get; set; }

        internal DetailMeasurementMeteringPointCharacteristic? DetailMeasurementMeteringPointCharacteristic { get; set; }

        internal string? Function { get; set; }

        internal string? MeteringPointDomainLocationIdentification { get; set; }
    }
}
