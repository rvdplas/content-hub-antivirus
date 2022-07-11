using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContentHub.AntivirusScanner.Application.Helpers;
using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Functions.UnitTests
{
    public class AntivirusFunctionsTests
    {
        private readonly string _callbackUrl = "https://localhost/callback";

        private readonly Mock<IVirusScannerService> _scannerServiceMock;
        private readonly Mock<ILogger> _loggerMock;

        public AntivirusFunctionsTests()
        {
            _scannerServiceMock = new Mock<IVirusScannerService>();
            _loggerMock = new Mock<ILogger>();
        }

        [Fact]
        public async Task ReceiveScanRequest_ValidMScanRequest_Should_ReturnOk()
        {
            // arrange
            var antivirusScanRequest = new AntivirusScanRequest(
                _callbackUrl,
                new List<string>
                {
                    "https://localhost/source/1",
                    "https://localhost/source/2",
                    "https://localhost/source/3"
                });

            var httpRequest = new FakeHttpRequest
            {
                Body = GenerateStreamFromString(JsonConvert.SerializeObject(antivirusScanRequest))
            };

            var sut = CreateServiceUnderTest();

            // act
            var result = await sut.ReceiveScanRequest(httpRequest);

            // assert
            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task ReceiveScanRequest_ServiceThrowsInvalidOperationException_Should_LogException()
        {
            // arrange
            var antivirusScanRequest = new AntivirusScanRequest(
                _callbackUrl,
                new List<string>
                {
                    "https://localhost/source/1",
                    "https://localhost/source/2",
                    "https://localhost/source/3"
                });

            var httpRequest = new FakeHttpRequest
            {
                Body = GenerateStreamFromString(JsonConvert.SerializeObject(antivirusScanRequest))
            };

            //await _virusScannerService.StartScanAsync(antivirusScanRequest);
            _scannerServiceMock.Setup(x => x.StartScanAsync(It.IsAny<AntivirusScanRequest>()))
                .Throws<InvalidOperationException>();

            var sut = CreateServiceUnderTest();

            // act
            await sut.ReceiveScanRequest(httpRequest);

            // assert
            _loggerMock.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<InvalidOperationException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        }

        [Fact]
        public async Task ReceiveScanRequest_ServiceThrowsException_Should_LogException()
        {
            // arrange
            var antivirusScanRequest = new AntivirusScanRequest(
                _callbackUrl,
                new List<string>
                {
                    "https://localhost/source/1",
                    "https://localhost/source/2",
                    "https://localhost/source/3"
                });

            var httpRequest = new FakeHttpRequest
            {
                Body = GenerateStreamFromString(JsonConvert.SerializeObject(antivirusScanRequest))
            };

            //await _virusScannerService.StartScanAsync(antivirusScanRequest);
            _scannerServiceMock.Setup(x => x.StartScanAsync(It.IsAny<AntivirusScanRequest>()))
                .Throws<Exception>();

            var sut = CreateServiceUnderTest();

            // act
            await sut.ReceiveScanRequest(httpRequest);

            // assert
            _loggerMock.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        }

        [Fact]
        public async Task ReceiveScanRequest_EmptyPayload_Should_ReturnBadRequest()
        {
            // arrange
            var httpRequest = new FakeHttpRequest();

            var sut = CreateServiceUnderTest();

            // act
            var result = await sut.ReceiveScanRequest(httpRequest);

            // assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ReceiveScanRequest_InvalidPayload_Should_ReturnInternalServerError()
        {
            // arrange
            var httpRequest = new FakeHttpRequest
            {
                Body = GenerateStreamFromString(JsonConvert.SerializeObject(new AntivirusScanResponse("", AntivirusResultHelper.CreateOkResult())))
            };

            var sut = CreateServiceUnderTest();

            // act
            var result = await sut.ReceiveScanRequest(httpRequest);

            // assert
            result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ProcessScanRequest_ValidQueueItem_Should_Process()
        {
            // arrange
            var request = new AntivirusScanRequest(
                _callbackUrl,
                new List<string>
                {
                    "https://localhost/source/1",
                    "https://localhost/source/2",
                    "https://localhost/source/3"
                });

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessScanRequest(JsonConvert.SerializeObject(request));

            // assert
            _scannerServiceMock.Verify(x => x.ProcessFileScanRequestAsync(It.IsAny<AntivirusScanRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessScanRequest_ServiceThrowsException_Should_LogException()
        {
            // arrange
            var request = new AntivirusScanRequest(
                _callbackUrl,
                new List<string>
                {
                    "https://localhost/source/1",
                    "https://localhost/source/2",
                    "https://localhost/source/3"
                });

            _scannerServiceMock.Setup(x => x.ProcessFileScanRequestAsync(It.IsAny<AntivirusScanRequest>()))
                .Throws<InvalidOperationException>();

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessScanRequest(JsonConvert.SerializeObject(request));

            // assert
            _loggerMock.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<InvalidOperationException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        }

        [Fact]
        public async Task ProcessScanResult_ValidResponse_Should_CallScannerService()
        {
            // arrange
            var response = new AntivirusScanResponse(
                _callbackUrl,
                AntivirusResultHelper.CreateOkResult());

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessScanResult(JsonConvert.SerializeObject(response));

            // assert
            _scannerServiceMock.Verify(x => x.ProcessResponseAsync(It.IsAny<AntivirusScanResponse>()), Times.Once);
        }

        [Fact]
        public async Task ProcessScanResult_ServiceThrowsException_Should_LogException()
        {
            // arrange
            var response = new AntivirusScanResponse(
                _callbackUrl,
                AntivirusResultHelper.CreateOkResult());

            _scannerServiceMock.Setup(x => x.ProcessResponseAsync(It.IsAny<AntivirusScanResponse>()))
                 .Throws<InvalidOperationException>();

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessScanResult(JsonConvert.SerializeObject(response));

            // assert
            _loggerMock.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<InvalidOperationException>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!), Times.Once);
        }

        private AntivirusFunctions CreateServiceUnderTest()
        {
            return new AntivirusFunctions(
                _scannerServiceMock.Object,
                _loggerMock.Object);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private class FakeHttpRequest : HttpRequest
        {
            public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                throw new System.NotImplementedException();
            }

            public override HttpContext HttpContext { get; }
            public override string Method { get; set; }
            public override string Scheme { get; set; }
            public override bool IsHttps { get; set; }
            public override HostString Host { get; set; }
            public override PathString PathBase { get; set; }
            public override PathString Path { get; set; }
            public override QueryString QueryString { get; set; }
            public override IQueryCollection Query { get; set; }
            public override string Protocol { get; set; }
            public override IHeaderDictionary Headers { get; }
            public override IRequestCookieCollection Cookies { get; set; }
            public override long? ContentLength { get; set; }
            public override string ContentType { get; set; }
            public override Stream Body { get; set; }
            public override bool HasFormContentType { get; }
            public override IFormCollection Form { get; set; }
        }
    }
}