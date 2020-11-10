using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Energinet.DataHub.SoapValidation.Dtos;

namespace Energinet.DataHub.SoapValidation.Helpers
{
    internal class ValidationController
    {
        private List<ValidationProblem> _problems;
        private Stack<XmlReaderSettings> _responsibleSettings;

        internal ValidationController()
        {
            _responsibleSettings = new Stack<XmlReaderSettings>();
            _problems = new List<ValidationProblem>();
        }

        internal void AddProblem(XmlReaderSettings settings, ValidationProblem problem)
        {
            if (IsResponsible(settings))
            {
                _problems.Add(problem);
            }
        }

        internal List<ValidationProblem> GetProblems()
        {
            return _problems;
        }

        internal void TakeResponsibility(XmlReaderSettings settings)
        {
            _responsibleSettings.Push(settings);
        }

        internal void AbdicateResposibility(XmlReaderSettings settings)
        {
            if (_responsibleSettings.Peek() != settings)
            {
                throw new InvalidOperationException("The supplied XML settings did not have the responsibilty of handling errors");
            }

            _responsibleSettings.Pop();
        }

        internal ValidationContext GetValidationContext(XmlReaderSettings settings)
        {
            return new ValidationContext(this, settings);
        }

        internal void RemoveProblemsCausedOnSameLineAndPosition(int lineNumber, int linePosition)
        {
            _problems.RemoveAll(x => x.LineNumber == lineNumber && x.LinePosition == linePosition);
        }

        private bool IsResponsible(XmlReaderSettings settings)
        {
            if (_responsibleSettings.Any())
            {
                return _responsibleSettings.Peek() == settings;
            }

            return true;
        }
    }
}
