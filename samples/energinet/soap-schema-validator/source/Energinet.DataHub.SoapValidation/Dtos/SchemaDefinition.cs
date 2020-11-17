﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Energinet.DataHub.SoapValidation.Resolvers;

namespace Energinet.DataHub.SoapValidation.Dtos
{
    internal class SchemaDefinition : DocumentDefinition
    {
        private readonly string _xsdDocumentName;
        private readonly IResourceLocatorStrategy _resourceLocatorStrategy;
        private readonly Dictionary<string, SchemaDefinition> _allowedSubSchemaDefinitions;

        public SchemaDefinition(string rootElement, string targetNamespace, string xsdDocumentName, IResourceLocatorStrategy resourceLocatorStrategy)
            : this(rootElement, targetNamespace, xsdDocumentName, resourceLocatorStrategy, new List<SchemaDefinition>())
        {
        }

        public SchemaDefinition(string rootElement, string targetNamespace, string xsdDocumentName, IResourceLocatorStrategy resourceLocatorStrategy, IEnumerable<SchemaDefinition> allowedSubSchemaDefinitions)
            : base(rootElement, targetNamespace)
        {
            _xsdDocumentName = xsdDocumentName;
            _resourceLocatorStrategy = resourceLocatorStrategy;
            _allowedSubSchemaDefinitions = GetAllowedSchemaDictionaries(allowedSubSchemaDefinitions);
        }

        internal SchemaDefinition[] SubDefinitions => _allowedSubSchemaDefinitions.Values.ToArray();

        internal XmlSchemaSet CreateXmlSchemaSet()
        {
            var schemaSet = new XmlSchemaSet { XmlResolver = new EmbeddedResourceLocator(_resourceLocatorStrategy) };
            schemaSet.Add(Namespace, _xsdDocumentName);

            return schemaSet;
        }

        internal SchemaDefinition GetSubSchemaDefinition(string rootElement, string namespaceUri)
        {
            return _allowedSubSchemaDefinitions[CreateIdentifier(rootElement, namespaceUri)];
        }

        internal bool ContainsSubSchemaDefinition(string rootElement, string namespaceUri)
        {
            return _allowedSubSchemaDefinitions.ContainsKey(CreateIdentifier(rootElement, namespaceUri));
        }

        private static Dictionary<string, SchemaDefinition> GetAllowedSchemaDictionaries(IEnumerable<SchemaDefinition> allowedSubSchemaDefinitions)
        {
            return allowedSubSchemaDefinitions
                .ToDictionary(k => CreateIdentifier(k.RootElement, k.Namespace), v => v);
        }
    }
}