using System;

namespace Energinet.DataHub.SoapValidation.Dtos
{
    internal class DocumentDefinition
    {
        public DocumentDefinition(string rootElement, string targetNamespace)
        {
            RootElement = rootElement;
            Namespace = targetNamespace;
            Identifier = CreateIdentifier(rootElement, targetNamespace);
        }

        public string RootElement { get; }

        public string Namespace { get; }

        public string Identifier
        {
            get;
        }

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

            if (obj is DocumentDefinition other)
            {
                return Equals(other);
            }

            return obj.GetType() == GetType() && Equals((DocumentDefinition)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RootElement, Namespace);
        }

        protected static string CreateIdentifier(string rootElement, string targetNamespace)
            => $"{rootElement}#{targetNamespace}";

        private bool Equals(DocumentDefinition other)
        {
            return RootElement == other.RootElement && Namespace == other.Namespace;
        }
    }
}
