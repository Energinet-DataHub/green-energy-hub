using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Energinet.DataHub.SoapValidation.Dtos;

namespace Energinet.DataHub.SoapValidation.Schemas
{
    internal class SchemaCollection : IEnumerable<SchemaDefinition>
    {
        private readonly SchemaDefinition[] _definitions;

        protected SchemaCollection(SchemaDefinition[] definitions)
        {
            _definitions = definitions;
        }

        internal delegate SchemaDefinition? SchemaDefinitionFinder(SchemaDefinition[] schemas, DocumentDefinition definition);

        public static SchemaCollection DataHub => new DataHubSchemaCollection();

        public static IEnumerable<SchemaDefinition> Find(
            bool searchSubDefinitions,
            DocumentDefinition definition)
        {
            SchemaDefinitionFinder finder = FindTopLevelDefinition;
            if (searchSubDefinitions)
            {
                finder = TraverseSubDefinitions;
            }

            var dataHubSchema = DataHub.Find(finder, definition);
            if (dataHubSchema != null)
            {
                yield return dataHubSchema;
            }
        }

        public IEnumerator<SchemaDefinition> GetEnumerator()
            => ((IEnumerable<SchemaDefinition>)_definitions).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Finds the <see cref="SchemaDefinition"/> matching the <see cref="DocumentDefinition"/>
        /// </summary>
        /// <param name="finder">Finder that can locate the schema definition</param>
        /// <param name="definition"><see cref="DocumentDefinition"/> to locate the matching schema</param>
        /// <returns><see cref="SchemaDefinition"/> for the <see cref="DocumentDefinition"/></returns>
        /// <exception cref="InvalidOperationException">More then one <see cref="SchemaDefinition"/> matched the <see cref="DocumentDefinition"/></exception>
        internal SchemaDefinition? Find(SchemaDefinitionFinder finder, DocumentDefinition definition)
            => finder(_definitions, definition);

        /// <summary>
        /// Finds the <see cref="SchemaDefinition"/> matching the <see cref="DocumentDefinition"/>
        /// </summary>
        /// <param name="definition"><see cref="DocumentDefinition"/> to locate the matching schema</param>
        /// <returns><see cref="SchemaDefinition"/> for the <see cref="DocumentDefinition"/></returns>
        /// <exception cref="InvalidOperationException">More then one <see cref="SchemaDefinition"/> matched the <see cref="DocumentDefinition"/></exception>
        internal SchemaDefinition? Find(DocumentDefinition definition)
            => Find(FindTopLevelDefinition, definition);

        private static SchemaDefinition? TraverseSubDefinitions(SchemaDefinition[] schemas, DocumentDefinition definition)
        {
            if (schemas.Length == 0)
            {
                return null;
            }

            return schemas.SingleOrDefault(definition.Equals) ??
                   TraverseSubDefinitions(schemas.SelectMany(s => s.SubDefinitions).ToArray(), definition);
        }

        private static SchemaDefinition? FindTopLevelDefinition(SchemaDefinition[] schemas, DocumentDefinition definition)
            => schemas.SingleOrDefault(definition.Equals);
    }
}
