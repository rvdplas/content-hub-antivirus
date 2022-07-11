using ContentHub.AntivirusScanner.Application.Helpers;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Helpers
{
    public class AntivirusResultHelperTests
    {
        [Fact]
        public void CreateOkResult_Should_CreateValidModel()
        {
            // arrange / act
            var model = AntivirusResultHelper.CreateOkResult();

            // assert
            model.Value.ShouldBe("Ok");
        }

        [Fact]
        public void CreateVirusFoundResult_Should_CreateValidModel()
        {
            // arrange / act
            var model = AntivirusResultHelper.CreateVirusFoundResult();

            // assert
            model.Value.ShouldBe("Malicious");
        }
    }
}