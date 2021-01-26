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
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Energinet.DataHub.SoapValidation.Dtos;
using GreenEnergyHub.TestHelpers.Traits;
using Xunit;

namespace Energinet.DataHub.SoapValidation.Tests.UnitTests
{
    [Trait(TraitNames.Category, TraitValues.UnitTest)]
    public class DataHubSchemaValidatorTests
    {
        private const string TestFileFolder = "Energinet.DataHub.SoapValidation.Tests.TestFiles";
        private const string RSM001Folder = "Rsm001";
        private const string RSM009Folder = "Rsm009";
        private const string RSM012Folder = "Rsm012";
        private const string RSM027Folder = "Rsm027";

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
        [InlineData("RSM027_invalid_missing_values.xml", RSM027Folder, false, RejectionReason.DoesNotRespectSchema)]
        [InlineData("RSM027_valid.xml", RSM027Folder, true, RejectionReason.None)]
        public async Task Validate_Rsm_answers_are_correct_based_on_input_stream(string inputFileName, string messageTypeFolder, bool expectSuccess, RejectionReason expectedRejectionReason)
        {
            // Assemble
            using (var stream = GetInputStream(inputFileName, messageTypeFolder))
            {
                var sut = new XmlSchemaValidator();

                // Act
                var result = await sut.ValidateStreamAsync(stream).ConfigureAwait(false);

                // Assert
                Assert.Equal(expectSuccess, result.IsSuccess);
                Assert.Equal(expectedRejectionReason, result.RejectionReason);
                Assert.Equal(0, stream.Position);
            }
        }

        private static Stream GetInputStream(string fileName, string messageTypeFolder)
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
