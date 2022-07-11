using System;
using System.Collections.Generic;
using System.Linq;
using ContentHub.AntivirusScanner.Application.Extensions;
using ContentHub.AntivirusScanner.Application.Models;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Extensions
{
    public class AntivirusScanRequestExtensionsTests
    {
        [Fact]
        public void IsValid_ValidModel_Should_BeValid()
        {
            // arrange
            var callbackUrl = new Uri("http://localhost/callback").ToString();
            var sourceUrls = new List<string>
            {
                new Uri("http://localhost/source/1").ToString(),
                new Uri("http://localhost/source/2").ToString(),
                new Uri("http://localhost/source/3").ToString()
            };

            var request = new AntivirusScanRequest(callbackUrl, sourceUrls);

            // act
            var result = request.IsValid();

            // assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void IsValid_InvalidCallbackUrl_Should_BeInvalid()
        {
            // arrange
            var callbackUrl = "localhost/callback";
            var sourceUrls = new List<string>
            {
                new Uri("http://localhost/source/1").ToString(),
                new Uri("http://localhost/source/2").ToString(),
                new Uri("http://localhost/source/3").ToString()
            };

            var request = new AntivirusScanRequest(callbackUrl, sourceUrls);

            // act
            var result = request.IsValid();

            // assert
            result.ShouldBeFalse();
        }


        [Fact]
        public void IsValid_EmptyCallbackUrl_Should_BeInvalid()
        {
            // arrange
            var callbackUrl = string.Empty;
            var sourceUrls = new List<string>
            {
                new Uri("http://localhost/source/1").ToString(),
                new Uri("http://localhost/source/2").ToString(),
                new Uri("http://localhost/source/3").ToString()
            };

            var request = new AntivirusScanRequest(callbackUrl, sourceUrls);

            // act
            var result = request.IsValid();

            // assert
            result.ShouldBeFalse();
        }
        [Fact]
        public void IsValid_WithoutSourceUrls_Should_BeInvalid()
        {
            // arrange
            var callbackUrl = new Uri("http://localhost/callback").ToString();
            var sourceUrls = Enumerable.Empty<string>().ToList();

            var request = new AntivirusScanRequest(callbackUrl, sourceUrls);

            // act
            var result = request.IsValid();

            // assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void IsValid_InvalidSourceUrl_Should_BeInvalid()
        {
            // arrange
            var callbackUrl = new Uri("http://localhost/callback").ToString();
            var sourceUrls = new List<string>
            {
                new Uri("http://localhost/source/1").ToString(),
                new Uri("http://localhost/source/2").ToString(),
                "localhost/source/3"
            };

            var request = new AntivirusScanRequest(callbackUrl, sourceUrls);

            // act
            var result = request.IsValid();

            // assert
            result.ShouldBeFalse();
        }
    }
}
