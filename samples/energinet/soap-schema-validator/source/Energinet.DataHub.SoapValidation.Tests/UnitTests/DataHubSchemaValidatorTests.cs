using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Energinet.DataHub.SoapValidation.Dtos;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.UnitTests
{
    public class DataHubSchemaValidatorTest
    {
        private const string TestFileFolder = "Energinet.DataHub.SoapValidation.Tests.TestFiles";
        private const string RSM001Folder = "Rsm001";
        private const string RSM009Folder = "Rsm009";
        private const string RSM012Folder = "Rsm012";

        [Theory]
        [InlineData("RSM001_CPR.xml", RSM001Folder, true, RejectionReason.None)]
        [InlineData("RSM001_CVR.xml", RSM001Folder, true, RejectionReason.None)]
        [InlineData("RSM001_Both.xml", RSM001Folder, true, RejectionReason.None)]
        [InlineData("ValidHourly.xml", RSM012Folder, true, RejectionReason.None)]
        [InlineData("ValidQuarterOfHour.xml", RSM012Folder, true, RejectionReason.None)]
        [InlineData("ValidBundledMessage.xml", RSM012Folder, true, RejectionReason.None)]
        [InlineData("RSM009_valid.xml", RSM009Folder, true, RejectionReason.None)]
        [InlineData("ValidHourlyButNoSOAP.xml", RSM012Folder, false, RejectionReason.SchemasUnavailable)]
        [InlineData("ValidSchemaButWrongRoot.xml", RSM012Folder, false, RejectionReason.SchemasUnavailable)]
        [InlineData("OldSchemaVersion.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("IncompleteXML.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("MissingMandatoryBusinessProcessRole.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("ExtraElement.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("ExtraElementInDataHubB2B.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("ExtraElementInSOAP.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("InvalidBundledMessageWithExtraElement.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("SOAPButNotB2B.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("SOAPAndB2BButNotEbiX.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("SOAPAndEbixButNoB2B.xml", RSM012Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("NotXML.xml", RSM012Folder, false, RejectionReason.InvalidXml)]
        [InlineData("InvalidXML.xml", RSM012Folder, false, RejectionReason.InvalidXml)]
        public async Task Validate_Rsm_answers_are_correct_based_on_input_stream(string inputFileName, string messageTypeFolder, bool expectSuccess, RejectionReason expectedRejectionReason)
        {
            // Assemble
            using (var stream = GetInputStream(inputFileName, messageTypeFolder))
            {
                var sut = new XmlSchemaValidator();

                // Act
                var result = await sut.ValidateStreamAsync(stream);

                // Assert
                Assert.Equal(expectSuccess, result.IsSuccess);
                Assert.Equal(expectedRejectionReason, result.RejectionReason);
                Assert.Equal(0, stream.Position);
            }
        }

        private Stream GetInputStream(string fileName, string messageTypeFolder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"{TestFileFolder}.{messageTypeFolder}.{fileName}";
            var result = assembly.GetManifestResourceStream(resourceName);
            if (result == null)
            {
                throw new NotImplementedException($"The filename {fileName} has not been added as an embedded resource to the project");
            }

            return result;
        }
    }
}
