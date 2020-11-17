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
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Google.Protobuf.WellKnownTypes;
using KellermanSoftware.CompareNetObjects;
using Protobuf.Implementations.Json;
using Protobuf.Implementations.Protobuf;
using Proto = SpikeProtobuf.Implementations.Protobuf;

namespace Protobuf
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [ShortRunJob]
    public class ProtobufVsJson
    {
        [Params(1, 100, 1000)]
        public int Items = 100;

        private Timeseries[] _jsonTimeSeries;
        private Proto.Timeseries[] _protobufTimeSeries;
        private Proto.TimeseriesCollection _protobufCollection;

        private MockFileSystem _protobufFileSystem;
        private MockFileSystem _protobufCollectionFileSystem;
        private MockFileSystem _jsonFileSystem;
        private MockFileSystem _jsonCollectionFileSystem;
        private MockFileSystem _jsonZipFileSystem;
        private MockFileSystem _jsonZipCollectionFileSystem;

        private static string S => Guid.NewGuid().ToString("N");

        [GlobalSetup]
        public async Task SetupAsync()
        {
            _jsonTimeSeries = CreateJsonTimeSeries(Items).ToArray();
            _protobufTimeSeries = CreateProtoTimeSeries(Items).ToArray();
            _protobufCollection = new Proto.TimeseriesCollection();
            _protobufCollection.Items.AddRange(_protobufTimeSeries);

            _protobufFileSystem = new MockFileSystem();
            IObjectSerializer<Proto.Timeseries> protobufWriter = new ProtobufTimeSeriesSerializer(_protobufFileSystem);
            await BenchmarkSerializeAsync(protobufWriter, _protobufTimeSeries);

            var protobufCollection = new Proto.TimeseriesCollection();
            protobufCollection.Items.AddRange(_protobufTimeSeries);
            _protobufCollectionFileSystem = new MockFileSystem();
            IObjectSerializer<Proto.TimeseriesCollection> protobufCollectionWriter = new ProtobufTimeSeriesSerializer(_protobufCollectionFileSystem);
            await BenchmarkSerializeAsync(protobufCollectionWriter, protobufCollection);

            _jsonFileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries> jsonWriter = new JsonTimeSeriesSerializer(_jsonFileSystem);
            await BenchmarkSerializeAsync(jsonWriter, _jsonTimeSeries);

            _jsonCollectionFileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries[]> jsonCollectionWriter = new JsonTimeSeriesSerializer(_jsonCollectionFileSystem);
            await BenchmarkSerializeAsync(jsonCollectionWriter, _jsonTimeSeries);

            _jsonZipFileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries> jsonZipWriter = new GZipJsonTimeSeriesSerializer(_jsonZipFileSystem);
            await BenchmarkSerializeAsync(jsonZipWriter, _jsonTimeSeries);

            _jsonZipCollectionFileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries[]> jsonZipCollectionWriter = new GZipJsonTimeSeriesSerializer(_jsonZipCollectionFileSystem);
            await BenchmarkSerializeAsync(jsonZipCollectionWriter, _jsonTimeSeries);

            // Manual sizes you can add to the markdown file
            var builder = new StringBuilder();
            builder.AppendLine("Size: " + Items.ToString());
            builder.AppendLine($"| protobufFileSystem | {GetSizeInKiloBytes(_protobufFileSystem)} |");
            builder.AppendLine($"| protobufCollectionFileSystem | {GetSizeInKiloBytes(_protobufCollectionFileSystem)} |");
            builder.AppendLine($"| jsonFileSystem | {GetSizeInKiloBytes(_jsonFileSystem)} |");
            builder.AppendLine($"| jsonCollectionFileSystem | {GetSizeInKiloBytes(_jsonCollectionFileSystem)} |");
            builder.AppendLine($"| jsonZipFileSystem | {GetSizeInKiloBytes(_jsonZipFileSystem)} |");
            builder.AppendLine($"| jsonZipCollectionFileSystem | {GetSizeInKiloBytes(_jsonZipCollectionFileSystem)} |");
            builder.AppendLine("|---|---|");
            await File.AppendAllTextAsync(@"c:\temp\sizes.txt", builder.ToString());
        }

        [BenchmarkCategory("Serialization", "Singles")]
        [Benchmark(Description = "Protobuf")]
        public async Task ProtobufSerializationAsync()
        {
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Proto.Timeseries> serializer = new ProtobufTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, _protobufTimeSeries);
        }

        [BenchmarkCategory("Serialization", "Singles")]
        [Benchmark(Description = "Json", Baseline = true)]
        public async Task JsonSerializationAsync()
        {
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries> serializer = new JsonTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, _jsonTimeSeries);
        }

        [BenchmarkCategory("Serialization", "Singles")]
        [Benchmark(Description = "Gzip")]
        public async Task JsonGzipSerializationAsync()
        {
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries> serializer = new GZipJsonTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, _jsonTimeSeries);
        }

        [BenchmarkCategory("Serialization", "Collection")]
        [Benchmark(Description = "Protobuf")]
        public async Task ProtobufCollectionSerializationAsync()
        {
            var protobufCollection = new Proto.TimeseriesCollection();
            protobufCollection.Items.AddRange(_protobufTimeSeries);
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Proto.TimeseriesCollection> serializer = new ProtobufTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, protobufCollection);
        }

        [BenchmarkCategory("Serialization", "Collection")]
        [Benchmark(Description = "Json", Baseline = true)]
        public async Task JsonCollectionSerializationAsync()
        {
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries[]> serializer = new JsonTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, _jsonTimeSeries);
        }

        [BenchmarkCategory("Serialization", "Collection")]
        [Benchmark(Description = "Gzip")]
        public async Task JsonGzipCollectionSerializationAsync()
        {
            var fileSystem = new MockFileSystem();
            IObjectSerializer<Timeseries[]> serializer = new GZipJsonTimeSeriesSerializer(fileSystem);
            await BenchmarkSerializeAsync(serializer, _jsonTimeSeries);
        }

        [BenchmarkCategory("Deserialization", "Singles")]
        [Benchmark(Description = "Protobuf")]
        public async Task<object> ProtobufDeserializationAsync()
        {
            IObjectSerializer<Proto.Timeseries> serializer = new ProtobufTimeSeriesSerializer(_protobufFileSystem);
            return await BenchmarkDeserializeAsync(_protobufFileSystem, serializer);
        }

        [BenchmarkCategory("Deserialization", "Singles")]
        [Benchmark(Description = "Json", Baseline = true)]
        public async Task<object> JsonDeserializationAsync()
        {
            IObjectSerializer<Timeseries> serializer = new JsonTimeSeriesSerializer(_jsonFileSystem);
            return await BenchmarkDeserializeAsync(_jsonFileSystem, serializer);
        }

        [BenchmarkCategory("Deserialization", "Singles")]
        [Benchmark(Description = "Gzip")]
        public async Task<object> JsonGzipDeserializationAsync()
        {
            IObjectSerializer<Timeseries> serializer = new GZipJsonTimeSeriesSerializer(_jsonZipFileSystem);
            return await BenchmarkDeserializeAsync(_jsonZipFileSystem, serializer);
        }

        [BenchmarkCategory("Deserialization", "Collection")]
        [Benchmark(Description = "Protobuf")]
        public async Task<object> ProtobufCollectionDeserializationAsync()
        {
            IObjectSerializer<Proto.TimeseriesCollection> serializer = new ProtobufTimeSeriesSerializer(_protobufCollectionFileSystem);
            return await BenchmarkDeserializeAsync(_protobufCollectionFileSystem, serializer);
        }

        [BenchmarkCategory("Deserialization", "Collection")]
        [Benchmark(Description = "Json", Baseline = true)]
        public async Task<object> JsonCollectionDeserializationAsync()
        {
            IObjectSerializer<Timeseries[]> serializer = new JsonTimeSeriesSerializer(_jsonCollectionFileSystem);
            return await BenchmarkDeserializeAsync(_jsonCollectionFileSystem, serializer);
        }

        [BenchmarkCategory("Deserialization", "Collection")]
        [Benchmark(Description = "Gzip")]
        public async Task<object> JsonGzipCollectionDeserializationAsync()
        {
            IObjectSerializer<Timeseries[]> serializer = new GZipJsonTimeSeriesSerializer(_jsonZipCollectionFileSystem);
            return await BenchmarkDeserializeAsync(_jsonZipCollectionFileSystem, serializer);
        }

        private async Task BenchmarkSerializeAsync<TType>(IObjectSerializer<TType> serializer, TType item)
            => await BenchmarkSerializeAsync(serializer, new[] { item });

        private async Task BenchmarkSerializeAsync<TType>(IObjectSerializer<TType> serializer, TType[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                await serializer.SerializeAsync(items[i]);
            }
        }

        private async Task<object> BenchmarkDeserializeAsync<TType>(MockFileSystem fileSystem, IObjectSerializer<TType> serializer)
        {
            var streams = fileSystem.AllFiles.Select(f => fileSystem.File.OpenRead(f));
            var results = new List<TType>();
            foreach (var stream in streams)
            {
                results.Add(await serializer.DeserializeAsync(stream));
            }

            return results;
        }

        private long GetSizeInKiloBytes(MockFileSystem fileSystem)
            => fileSystem.AllFiles.Sum(f => fileSystem.FileInfo.FromFileName(f).Length) / 1024;

        public static async Task VerifySerializersAsync()
        {
            var jsonTimeSeries = CreateJsonTimeSeries().ToArray();
            var protobufTimeSeries = CreateProtoTimeSeries().ToArray();

            var protobufFileSystem = new MockFileSystem();
            IObjectSerializer<SpikeProtobuf.Implementations.Protobuf.Timeseries> protobufWriter = new ProtobufTimeSeriesSerializer(protobufFileSystem);
            await VerifySerializerAsync(protobufFileSystem, protobufWriter, protobufTimeSeries.First());

            var protobufCollection = new SpikeProtobuf.Implementations.Protobuf.TimeseriesCollection();
            protobufCollection.Items.AddRange(protobufTimeSeries);
            var protoCollFileSystem = new MockFileSystem();
            var protoCollWriter = new ProtobufTimeSeriesSerializer(protoCollFileSystem);
            await VerifySerializerAsync(protoCollFileSystem, protoCollWriter, protobufCollection);

            var jsonFileSystem = new MockFileSystem();
            var jsonWriter = new JsonTimeSeriesSerializer(jsonFileSystem);
            await VerifySerializerAsync(jsonFileSystem, jsonWriter, jsonTimeSeries.First());

            var jsonArrFileSystem = new MockFileSystem();
            var jsonArrWriter = new JsonTimeSeriesSerializer(jsonArrFileSystem);
            await VerifySerializerAsync(jsonArrFileSystem, jsonArrWriter, jsonTimeSeries.First());

            var zipFileSystem = new MockFileSystem();
            var gzipWriter = new GZipJsonTimeSeriesSerializer(zipFileSystem);
            await VerifySerializerAsync(zipFileSystem, gzipWriter, jsonTimeSeries.First());

            var zipArrFileSystem = new MockFileSystem();
            var gzipArrWriter = new GZipJsonTimeSeriesSerializer(zipArrFileSystem);
            await VerifySerializerAsync(zipArrFileSystem, gzipArrWriter, jsonTimeSeries.First());

            Console.WriteLine(string.Empty);
            Console.WriteLine("Serialized data verified");
        }

        private static async Task VerifySerializerAsync<T>(MockFileSystem fileSystem, IObjectSerializer<T> serializer, T model)
        {
            var comparer = new CompareLogic();
            await serializer.SerializeAsync(model);
            var fileStream = fileSystem.File.OpenRead(fileSystem.AllFiles.Single());
            var fromFileSystem = await serializer.DeserializeAsync(fileStream);
            var comparisonResult = comparer.Compare(model, fromFileSystem);
            if (!comparisonResult.AreEqual)
            {
                throw new Exception("Error in serialize/deserialize");
            }
        }

        private static IEnumerable<SpikeProtobuf.Implementations.Protobuf.Timeseries> CreateProtoTimeSeries(int count = 100)
        {
            SpikeProtobuf.Implementations.Protobuf.Timeseries Timeseries()
            {
                var timeseries = new SpikeProtobuf.Implementations.Protobuf.Timeseries
                {
                    Function = S,
                    ObservationPeriod = new SpikeProtobuf.Implementations.Protobuf.Timeseries.Types.Period
                    {
                        Resolution = S,
                        Start = Timestamp.FromDateTime(DateTime.UtcNow),
                        End = Timestamp.FromDateTime(DateTime.UtcNow)
                    },
                    Product = new SpikeProtobuf.Implementations.Protobuf.Timeseries.Types.ProductType
                    {
                        Identification = S,
                        UnitType = S
                    },
                    TransactionId = S,
                    MeteringPointCharacteristic = new SpikeProtobuf.Implementations.Protobuf.Timeseries.Types.DetailMeasurementMeteringPointCharacteristic
                    {
                        SettlementMethod = S,
                        TypeOfMeteringPoint = S
                    },
                    MeteringPointDomainLocation = S
                };
                timeseries.Observations.AddRange(ManyOf(96, i => new SpikeProtobuf.Implementations.Protobuf.Timeseries.Types.Observation
                {
                    Missing = false,
                    Position = i,
                    Quality = S,
                    Quantity = 10
                }));
                return timeseries;
            }

            for (var i = 0; i < count; i++)
            {
                yield return Timeseries();
            }
        }

        private static IEnumerable<Timeseries> CreateJsonTimeSeries(int count = 100)
        {
            Timeseries Timeseries()
            {
                var obj = new Timeseries
                {
                    Function = S,
                    Period = new Period
                    {
                        Start = DateTime.Now,
                        End = DateTime.Now,
                        Resolution = S
                    },
                    Product = new Product
                    {
                        Identification = S,
                        UnitType = S
                    },
                    TransactionId = S,
                    MeteringPointCharacteristic = new DetailMeasurementMeteringPointCharacteristic
                    {
                        SettlementMethod = S,
                        TypeOfMeteringPoint = S
                    },
                    MeteringPointDomainLocation = S,
                    Observations = ManyOf(96, i => new Observation
                    {
                        Missing = false,
                        Position = i,
                        Quality = S,
                        Quantity = 10
                    })
                };
                return obj;
            }

            for (var i = 0; i < count; i++)
            {
                yield return Timeseries();
            }
        }

        private static T[] ManyOf<T>(int count, Func<T> producer)
        {
            var p = producer;
            return ManyOf(count, i => p());
        }

        private static T[] ManyOf<T>(int count, Func<int, T> producer)
        {
            var array = new T[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = producer(i);
            }

            return array;
        }
    }
}
