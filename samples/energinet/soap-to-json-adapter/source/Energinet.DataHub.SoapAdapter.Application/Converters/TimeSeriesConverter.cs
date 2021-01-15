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
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.SoapAdapter.Application.Converters.Fragments.RSM012;
using Energinet.DataHub.SoapAdapter.Application.Converters.Iso8601;
using Energinet.DataHub.SoapAdapter.Application.Parsers;
using Energinet.DataHub.SoapAdapter.Domain.Validation;
using NodaTime;
using NodaTime.Text;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    public class TimeSeriesConverter : RsmConverter
    {
        private const string B2BNamespace = "un:unece:260:data:EEM-DK_MeteredDataTimeSeries:v3";

        protected override async ValueTask ConvertPayloadAsync(XmlReader reader, RsmHeader header, Utf8JsonWriter writer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (header == null)
            {
                throw new ArgumentNullException(nameof(header));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteStartArray();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is("PayloadEnergyTimeSeries", B2BNamespace))
                {
                    await ProcessPayloadEnergyTimeSeriesAsync(reader, header, writer)
                        .ConfigureAwait(false);
                }

                if (reader.Is("DK_MeteredDataTimeSeries", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
            }

            writer.WriteEndArray();
        }

        private static async ValueTask<string> GetChildIdentificationAsync(XmlReader reader)
        {
            if (reader.ReadToFollowing("Identification", B2BNamespace))
            {
                return await reader.ReadElementContentAsStringAsync()
                    .ConfigureAwait(false);
            }

            return string.Empty;
        }

        private static async ValueTask ProcessPayloadEnergyTimeSeriesAsync(XmlReader reader, RsmHeader header, Utf8JsonWriter writer)
        {
            var fragments = new RSM012Fragments();

            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                if (reader.Is("PayloadEnergyTimeSeries", B2BNamespace))
                {
                    writer.WriteStartObject();
                }
                else if (reader.Is("Identification", B2BNamespace))
                {
                    fragments.PayloadEnergyTimeSeriesIdentification = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("Function", B2BNamespace))
                {
                    fragments.Function = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("ObservationTimeSeriesPeriod", B2BNamespace))
                {
                    fragments.ObservationTimeSeriesPeriod = await ProcessObservationTimeSeriesPeriodAsync(reader)
                        .ConfigureAwait(false);
                }
                else if (reader.Is("IncludedProductCharacteristic", B2BNamespace))
                {
                   fragments.IncludedProductCharacteristic = await ProcessIncludedProductCharacteristicAsync(reader)
                       .ConfigureAwait(false);
                }
                else if (reader.Is("DetailMeasurementMeteringPointCharacteristic", B2BNamespace))
                {
                    fragments.DetailMeasurementMeteringPointCharacteristic = await ProcessDetailMeasurementMeteringPointCharacteristicAsync(reader)
                        .ConfigureAwait(false);
                }
                else if (reader.Is("MeteringPointDomainLocation", B2BNamespace))
                {
                    fragments.MeteringPointDomainLocationIdentification = await GetChildIdentificationAsync(reader)
                        .ConfigureAwait(false);

                    /* 2020-12-22 - XMDJE - At this point we have all the necessary information to write everything to the stream up until points.
                        Points are specifically written immediately to the stream in the effort to save memory.
                        This means we will write everything up to that segment to the stream to be ready */
                    WriteTimeSeriesUpUntilIndividualPoints(writer, fragments, header);
                }
                else if (reader.Is("IntervalEnergyObservation", B2BNamespace))
                {
                    // Notice: We write each point to the stream immediately, unlike the rest of the parsing
                    var point = await ProcessPointsAsync(reader)
                        .ConfigureAwait(false);
                    WriteTimeSeriesPoint(writer, point, fragments);
                }
                else if (reader.Is("PayloadEnergyTimeSeries", B2BNamespace, XmlNodeType.EndElement))
                {
                    // We have now parsed all the points and can write the remaining parts of the document
                    WriteTimeSeriesFromAfterPoints(writer, fragments);
                    writer.WriteEndObject();
                }
            }
            while (await reader.ReadAsync().ConfigureAwait(false));
        }

        private static async ValueTask<ObservationTimeSeriesPeriod> ProcessObservationTimeSeriesPeriodAsync(XmlReader reader)
        {
            var resolutionDuration = string.Empty;
            var start = Instant.MinValue;
            var end = Instant.MinValue;

            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                if (reader.Is("ResolutionDuration", B2BNamespace))
                {
                    resolutionDuration = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("Start", B2BNamespace))
                {
                    var startParseResult =
                        InstantPattern.General.Parse(await reader.ReadElementContentAsStringAsync()
                            .ConfigureAwait(false));
                    if (startParseResult.Success)
                    {
                        start = startParseResult.Value;
                    }
                }
                else if (reader.Is("End", B2BNamespace))
                {
                    var endParseResult =
                        InstantPattern.General.Parse(await reader.ReadElementContentAsStringAsync()
                            .ConfigureAwait(false));
                    if (endParseResult.Success)
                    {
                        end = endParseResult.Value;
                    }
                }
                else if (reader.Is("ObservationTimeSeriesPeriod", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
            }
            while (await reader.ReadAsync()
                .ConfigureAwait(false));

            var observationTimeSeriesPeriod = new ObservationTimeSeriesPeriod(resolutionDuration, start, end);
            return observationTimeSeriesPeriod;
        }

        private static async ValueTask<IncludedProductCharacteristic> ProcessIncludedProductCharacteristicAsync(XmlReader reader)
        {
            var identification = string.Empty;
            var unitType = string.Empty;

            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                if (reader.Is("Identification", B2BNamespace))
                {
                    identification = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("UnitType", B2BNamespace))
                {
                    unitType = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("IncludedProductCharacteristic", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
            }
            while (await reader.ReadAsync().ConfigureAwait(false));

            var includedProductCharacteristic =
                new IncludedProductCharacteristic(identification, unitType);
            return includedProductCharacteristic;
        }

        private static async ValueTask<DetailMeasurementMeteringPointCharacteristic> ProcessDetailMeasurementMeteringPointCharacteristicAsync(XmlReader reader)
        {
            var typeOfMeteringPoint = string.Empty;
            var settlementMethod = string.Empty;

            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                if (reader.Is("TypeOfMeteringPoint", B2BNamespace))
                {
                    typeOfMeteringPoint = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("SettlementMethod", B2BNamespace))
                {
                    settlementMethod = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("DetailMeasurementMeteringPointCharacteristic", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
            }
            while (await reader.ReadAsync().ConfigureAwait(false));

            var detailMeasurementMeteringPointCharacteristic =
                new DetailMeasurementMeteringPointCharacteristic(typeOfMeteringPoint, settlementMethod);
            return detailMeasurementMeteringPointCharacteristic;
        }

        private static async Task<TimeSeriesPoint> ProcessPointsAsync(XmlReader reader)
        {
            var point = new TimeSeriesPoint();
            do
            {
                if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
                {
                    continue;
                }

                // TODO Add support for the calculated "Time" field, see TimeSeriesSplitter
                if (reader.Is("Position", B2BNamespace))
                {
                    var value = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                    point.Position = int.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (reader.Is("EnergyQuantity", B2BNamespace))
                {
                    // Note: If EnergyQuantity is there, QuantiyMissing is not
                    var value = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                    point.EnergyQuantity = decimal.Parse(value, CultureInfo.InvariantCulture);
                }
                else if (reader.Is("QuantityQuality", B2BNamespace))
                {
                    point.QuantityQuality = await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (reader.Is("QuantityMissing", B2BNamespace))
                {
                    // Note: If the QuantityMissing element is there, EnergyQuantity is not and QuantityQuality is irrelevant
                    await reader.ReadElementContentAsStringAsync()
                        .ConfigureAwait(false);

                    // If QuantityMissing is there, it means the quality is missing (regardless of value), which is mapped to a CIM value
                    point.QuantityMissing = true;
                }
                else if (reader.Is("IntervalEnergyObservation", B2BNamespace, XmlNodeType.EndElement))
                {
                    break;
                }
            }
            while (await reader.ReadAsync().ConfigureAwait(false));

            return point;
        }

        private static void WriteTimeSeriesPoint(
            Utf8JsonWriter writer, TimeSeriesPoint point, RSM012Fragments fragments)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Position", point.Position);
            writer.WriteNumber("Quantity", point.GetCimEnergyQuantity());
            writer.WriteString("Quality", point.GetCimQuantityQuality());
            writer.WriteString(
                "Time",
                Iso8601Duration.GetObservationTime(
                    fragments.ObservationTimeSeriesPeriod?.Start,
                    fragments.ObservationTimeSeriesPeriod?.ResolutionDuration,
                    point.Position).ToString());

            writer.WriteEndObject();
        }

        private static void WriteTimeSeriesUpUntilIndividualPoints(Utf8JsonWriter writer, RSM012Fragments fragments, RsmHeader header)
        {
            WriteIdentifications(writer, header, fragments);
            WriteMarketDocument(writer, header);
            WriteActivityRecordStatus(writer, fragments);
            WriteCharacteristics(writer, fragments);
            WriteMarketEvaluationPointMRID(writer, fragments);
            WriteCorrelationId(writer);
            WritePeriodUpToPoints(writer, fragments);
        }

        private static void WriteIdentifications(Utf8JsonWriter writer, RsmHeader header, RSM012Fragments fragments)
        {
            writer.WriteString("mRID", fragments.PayloadEnergyTimeSeriesIdentification);
            writer.WriteString("MessageReference", header.MessageReference);
        }

        private static void WriteMarketDocument(Utf8JsonWriter writer, RsmHeader header)
        {
            writer.WriteStartObject("MarketDocument");

            writer.WriteString("mRID", header.Identification);
            writer.WriteString("Type", header.DocumentType);
            writer.WriteString("CreatedDateTime", header.Creation.ToString());

            WriteMarketParticipant(
                writer,
                "SenderMarketParticipant",
                header.SenderIdentification,
                "VA",
                header.EnergyBusinessProcessRole);
            WriteMarketParticipant(
                writer,
                "RecipientMarketParticipant",
                header.RecipientIdentification,
                "VA",
                null);

            writer.WriteString("ProcessType", header.EnergyBusinessProcess);
            writer.WriteString("MarketServiceCategory_Kind", header.EnergyIndustryClassification);

            writer.WriteEndObject();
        }

        private static void WriteMarketParticipant(Utf8JsonWriter writer, string objectName, string? mrid, string? qualifier, string? type)
        {
            writer.WriteStartObject(objectName);

            writer.WriteString("mRID", mrid);
            writer.WriteString("qualifier", qualifier);
            writer.WriteNull("name");
            if (type == null)
            {
                writer.WriteNull("Type");
            }
            else
            {
                writer.WriteString("Type", type);
            }

            writer.WriteEndObject();
        }

        private static void WriteActivityRecordStatus(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteString("MktActivityRecord_Status", fragments.Function);
        }

        private static void WriteCharacteristics(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteString("Product", fragments.IncludedProductCharacteristic!.Identification);
            writer.WriteString("QuantityMeasurementUnit_Name", fragments.IncludedProductCharacteristic.UnitType);
            writer.WriteString("MarketEvaluationPointType", fragments.DetailMeasurementMeteringPointCharacteristic!.TypeOfMeteringPoint);
            writer.WriteString("SettlementMethod", fragments.DetailMeasurementMeteringPointCharacteristic.SettlementMethod);
        }

        private static void WriteMarketEvaluationPointMRID(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteString("MarketEvaluationPoint_mRID", fragments.MeteringPointDomainLocationIdentification);
        }

        private static void WriteCorrelationId(Utf8JsonWriter writer)
        {
            writer.WriteString("CorrelationId", "Unknown"); // TODO Figure out where correlation ID comes from
        }

        private static void WritePeriodUpToPoints(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteStartObject("Period");

            writer.WriteString("Resolution", fragments.ObservationTimeSeriesPeriod!.ResolutionDuration);

            WriteTimeInterval(writer, fragments);

            WriteStartOfPoints(writer);
        }

        private static void WriteTimeInterval(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteStartObject("TimeInterval");

            writer.WriteString("Start", fragments.ObservationTimeSeriesPeriod!.Start.ToString());
            writer.WriteString("End", fragments.ObservationTimeSeriesPeriod!.End.ToString());

            writer.WriteEndObject();
        }

        private static void WriteStartOfPoints(Utf8JsonWriter writer)
        {
            writer.WriteStartArray("Points");
        }

        private static void WriteTimeSeriesFromAfterPoints(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            WriteEndOfPoints(writer);
            WriteTransaction(writer, fragments);
            WriteRequestDate(writer);
        }

        private static void WriteEndOfPoints(Utf8JsonWriter writer)
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static void WriteTransaction(Utf8JsonWriter writer, RSM012Fragments fragments)
        {
            writer.WriteStartObject("Transaction");

            writer.WriteString("mRID", fragments.PayloadEnergyTimeSeriesIdentification);

            writer.WriteEndObject();
        }

        private static void WriteRequestDate(Utf8JsonWriter writer)
        {
            Instant now = SystemClock.Instance.GetCurrentInstant(); // TODO Consider IClock or getting the request date from elsewhere
            writer.WriteString("RequestDate", now.ToString());
        }
    }
}
