using AutoFixture;
using ContentHub.AntivirusScanner.Application.Helpers;
using ContentHub.AntivirusScanner.Application.Models;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Models
{
    public class AntivirusScanResponseTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public void Constructor_Should_InitializeProperties()
        {
            // arrange
            var callbackUrl = _fixture.Create<string>();
            var antivirusScanResponse = AntivirusResultHelper.CreateOkResult();

            // act
            var result = new AntivirusScanResponse(callbackUrl, antivirusScanResponse);

            // assert
            result.CallbackUrl.ShouldBe(callbackUrl);
            result.Response.ShouldBe(antivirusScanResponse);
        }
    }
}
