using System.Collections.Generic;
using AutoFixture;
using ContentHub.AntivirusScanner.Application.Models;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Models
{
    public class AntivirusScanRequestTests
    {
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public void Constructor_Should_InitializeProperties()
        {
            // arrange
            var callbackUrl = _fixture.Create<string>();
            var sources = _fixture.Create<List<string>>();

            // act
            var result = new AntivirusScanRequest(callbackUrl, sources);

            // assert
            result.CallbackUrl.ShouldBe(callbackUrl);
            result.Sources.ShouldBeEquivalentTo(sources);
        } 
    }
}
