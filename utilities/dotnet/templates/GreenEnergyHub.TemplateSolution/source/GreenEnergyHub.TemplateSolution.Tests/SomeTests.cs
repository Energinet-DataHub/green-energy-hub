using Xunit;

namespace GreenEnergyHub.TemplateSolution.Tests
{
    public class SomeTests
    {
        [Fact]
        public void DummySuccessfulTest()
        {
            // Write clever tests (not this one) to include in the template
            var sut = nameof(DummySuccessfulTest);

            Assert.Equal("DummySuccessfulTest", sut);
        }
    }
}