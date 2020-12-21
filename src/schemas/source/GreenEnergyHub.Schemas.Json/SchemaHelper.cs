﻿// Copyright 2020 Energinet DataHub A/S
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
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Json.Schema;

namespace GreenEnergyHub.Schemas.Json
{
    internal static class SchemaHelper
    {
        private static readonly Assembly _asm = typeof(SchemaHelper).Assembly;

        static SchemaHelper()
        {
            SchemaRegistry.Global.Register(new Uri("https://github.com/green-energy-hub/schemas"), SchemaHelper.CimDefinitions);
        }

        /// <summary>
        /// An array of found schemas
        /// </summary>
        internal static IEnumerable<string> Schemas { get; } = _asm.GetManifestResourceNames().Where(resourceName =>
            resourceName.EndsWith("schema.json", StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// CIM definitions
        /// </summary>
        internal static JsonSchema? CimDefinitions =>
            GetSchema("GreenEnergyHub.Schemas.Json.Schemas.cim-definitions.schema.json");

        /// <summary>
        /// Change of supplier
        /// </summary>
        internal static JsonSchema? InitiateChangeSupplier =>
            GetSchema("GreenEnergyHub.Schemas.Json.Schemas.ChangeOfSupplier.schema.json");

        /// <summary>
        /// Get a <see cref="JsonSchema"/> for a <see cref="SchemaType"/>
        /// </summary>
        /// <param name="schemaType">Type to locate</param>
        /// <returns><see cref="JsonSchema"/> for the type</returns>
        internal static JsonSchema? GetSchema(SchemaType schemaType)
        {
            return schemaType.Name switch
            {
                SchemaTypes.InitiateChangeSupplier => InitiateChangeSupplier,
                _ => null
            };
        }

        /// <summary>
        /// Get a <see cref="JsonSchema"/> from an embedded resource
        /// </summary>
        /// <param name="schema">Schema to locate</param>
        /// <returns>A <see cref="JsonSchema"/></returns>
        internal static JsonSchema? GetSchema([NotNull] string schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (string.IsNullOrEmpty(schema))
            {
                throw new ArgumentException("Schema is null or empty");
            }

            try
            {
                using var manifestResourceStream = _asm.GetManifestResourceStream(schema);
                if (manifestResourceStream == null)
                {
                    return null;
                }

                using var fr = new StreamReader(manifestResourceStream);

                return JsonSchema.FromText(fr.ReadToEnd());
            }
            catch (FileLoadException)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (BadImageFormatException)
            {
                return null;
            }
        }
    }
}