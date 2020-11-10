using System.Xml;
using Energinet.DataHub.SoapValidation.Dtos;
using Energinet.DataHub.SoapValidation.Helpers;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.UnitTests.Helpers
{
    public class ValidationControllerTest
    {
        private const int ProblemLineNumber = 1;
        private const int ProblemPositionNumber = 1;
        private const int SecondaryProblemLineNumber = 1;
        private const int SecondaryProblemPositionNumber = 2;

        [Fact]
        public void AddProblem_IfNoResponsible_AddProblem()
        {
            // Arrange
            var problem = GetValidationProblem();
            var settings = GetSettings();

            var sut = new ValidationController();

            // Act
            sut.AddProblem(settings, problem);

            // Assert
            Assert.Single(sut.GetProblems());
            Assert.Contains<ValidationProblem>(problem, sut.GetProblems());
        }

        [Fact]
        public void AddProblem_WhenResponsible_AddProblem()
        {
            // Arrange
            var problem = GetValidationProblem();
            var settings = GetSettings();

            var sut = new ValidationController();

            // Act
            sut.TakeResponsibility(settings);
            sut.AddProblem(settings, problem);

            // Assert
            Assert.Single(sut.GetProblems());
            Assert.Contains<ValidationProblem>(problem, sut.GetProblems());
        }

        [Fact]
        public void AddProblem_WhenNotResponsible_DoNotAddProblem()
        {
            // Arrange
            var problem = GetValidationProblem();
            var settings = GetSettings();
            var responsibleSettings = GetSettings();

            var sut = new ValidationController();

            // Act
            sut.TakeResponsibility(responsibleSettings);
            sut.AddProblem(settings, problem);

            // Assert
            Assert.Empty(sut.GetProblems());
        }

        [Fact]
        public void RemoveProblemsCausedOnSameLineAndPosition_WhenNoProblems_ListIsEmpty()
        {
            // Arrange
            var sut = new ValidationController();

            // Act
            sut.RemoveProblemsCausedOnSameLineAndPosition(ProblemLineNumber, ProblemPositionNumber);
            var result = sut.GetProblems();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveProblemsCausedOnSameLineAndPosition_WhenNoMatchingProblemExist_NothingIsRemoved()
        {
            // Arrange
            var settings = GetSettings();
            var problem = GetValidationProblem(SecondaryProblemLineNumber, SecondaryProblemPositionNumber);

            var sut = new ValidationController();
            sut.TakeResponsibility(settings);
            sut.AddProblem(settings, problem);

            // Act
            sut.RemoveProblemsCausedOnSameLineAndPosition(ProblemLineNumber, ProblemPositionNumber);

            // Assert
            Assert.Single(sut.GetProblems());
        }

        [Fact]
        public void RemoveProblemsCausedOnSameLineAndPosition_WhenMatchingProblemsExist_MatchingProblemsAreRemoved()
        {
            // Arrange
            var settings = GetSettings();
            var firstProblem = GetValidationProblem();
            var secondProblem = GetValidationProblem();
            var thirdProblem = GetValidationProblem(SecondaryProblemLineNumber, SecondaryProblemPositionNumber);

            var sut = new ValidationController();

            // Act
            sut.TakeResponsibility(settings);
            sut.AddProblem(settings, firstProblem);
            sut.AddProblem(settings, secondProblem);
            sut.AddProblem(settings, thirdProblem);

            sut.RemoveProblemsCausedOnSameLineAndPosition(ProblemLineNumber, ProblemPositionNumber);

            // Assert
            Assert.Single(sut.GetProblems());
        }

        private ValidationProblem GetValidationProblem(int lineNumber = ProblemLineNumber, int positionNumber = ProblemPositionNumber)
        {
            return new ValidationProblem(string.Empty, System.Xml.Schema.XmlSeverityType.Error, lineNumber, positionNumber);
        }

        private XmlReaderSettings GetSettings()
        {
            return new XmlReaderSettings();
        }
    }
}
