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
using System.Linq;

namespace TimeSeriesGeneratorApp
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this ICollection<T> self, int chunkSize)
        {
            var splitList = new List<List<T>>();
            var chunkCount = (int)Math.Ceiling(self.Count / (double)chunkSize);

            for (var c = 0; c < chunkCount; c++)
            {
                var skip = c * chunkSize;
                var take = skip + chunkSize;
                var chunk = new List<T>(chunkSize);

                for (var e = skip; e < take && e < self.Count; e++)
                {
                    chunk.Add(self.ElementAt(e));
                }

                splitList.Add(chunk);
            }

            return splitList;
        }
    }
}
