using System;
using System.Xml;

namespace Energinet.DataHub.SoapValidation.Helpers
{
    internal class ValidationContext : IDisposable
    {
        private ValidationController _controller;
        private XmlReaderSettings _settings;

        internal ValidationContext(ValidationController controller, XmlReaderSettings settings)
        {
            _settings = settings;
            _controller = controller;
            _controller.TakeResponsibility(settings);
        }

        public void Dispose()
        {
            _controller.AbdicateResposibility(_settings);
        }
    }
}
