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

using System.IO;
using System.Threading.Tasks;

namespace Energinet.DataHub.SoapAdapter.Application.Converters
{
    /// <summary>
    /// SendMessage response converter
    /// </summary>
    public interface IResponseConverter
    {
        /// <summary>
        /// Convert the input to another format
        /// </summary>
        /// <param name="input">A <see cref="Stream"/> that should be converted</param>
        /// <param name="output">A <see cref="Stream"/> that the converted data should be written to</param>
        /// <param name="identifier">A <see cref="string"/> identifying the response</param>
        ValueTask ConvertAsync(Stream input, Stream output, string identifier);
    }
}
