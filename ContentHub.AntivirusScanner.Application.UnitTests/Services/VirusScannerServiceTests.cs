using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using ContentHub.AntivirusScanner.Application.Helpers;
using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;
using ContentHub.AntivirusScanner.Application.Services;
using Moq;
using Moq.Protected;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Services
{
    public class VirusScannerServiceTests
    {
        private readonly IFixture _fixture = new Fixture();

        private readonly Mock<IScanRequestQueueRepository> _requestQueueRepository;
        private readonly Mock<IScanResponseQueueRepository> _responseQueueRepository;
        private readonly Mock<IOnlineScannerService> _cloudmersiveVirusScanner;
        private readonly Mock<IHttpClientFactory> _httpClientFactory;

        public VirusScannerServiceTests()
        {
            _requestQueueRepository = new Mock<IScanRequestQueueRepository>();
            _responseQueueRepository = new Mock<IScanResponseQueueRepository>();
            _cloudmersiveVirusScanner = new Mock<IOnlineScannerService>();
            _httpClientFactory = new Mock<IHttpClientFactory>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(Array.Empty<byte>())
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(client);
        }

        [Fact]
        public async Task ProcessFileScanRequestAsync_Should_StoreMessageAsync()
        {
            // arrange
            var request = new AntivirusScanRequest(
                $"https://localhost/{_fixture.Create<string>()}",
                new List<string>
                {
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                }
            );

            _cloudmersiveVirusScanner.Setup(x => x.IsFileVirusFree(It.IsAny<Stream>()))
                .ReturnsAsync(true);
            
            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessFileScanRequestAsync(request);

            // assert
            _responseQueueRepository.Verify(x => x.StoreMessageAsync(It.IsAny<AntivirusScanResponse>()), Times.Once);
        }

        [Fact]
        public async Task ProcessFileScanRequestAsync_Should_CheckEachSource()
        {
            // arrange
            var request = new AntivirusScanRequest(
                $"https://localhost/{_fixture.Create<string>()}",
                new List<string>
                {
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                }
            );

            _cloudmersiveVirusScanner.Setup(x => x.IsFileVirusFree(It.IsAny<Stream>()))
                .ReturnsAsync(true);

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessFileScanRequestAsync(request);

            // assert
            _cloudmersiveVirusScanner.Verify(x => x.IsFileVirusFree(It.IsAny<Stream>()), Times.Exactly(request.Sources.Count));
        }

        [Fact]
        public async Task ProcessFileScanRequestAsync_VirusNotFound_Should_ReportVirusFree()
        {
            // arrange
            var request = new AntivirusScanRequest(
                $"https://localhost/{_fixture.Create<string>()}",
                new List<string>
                {
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                }
            );

            _cloudmersiveVirusScanner.Setup(x => x.IsFileVirusFree(It.IsAny<Stream>()))
                .ReturnsAsync(true);

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessFileScanRequestAsync(request);

            // assert
            _responseQueueRepository.Verify(
                x => x.StoreMessageAsync(
                    It.Is<AntivirusScanResponse>(r => r.Response.Value.Equals(AntivirusResultHelper.VirusFreeCode))),
                Times.Once);
        }

        [Fact]
        public async Task ProcessFileScanRequestAsync_VirusFound_Should_ReportVirusFound()
        {
            // arrange
            var request = new AntivirusScanRequest(
                $"https://localhost/{_fixture.Create<string>()}",
                new List<string>
                {
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                    $"https://localhost/file/{_fixture.Create<string>()}",
                }
            );

            _cloudmersiveVirusScanner.Setup(x => x.IsFileVirusFree(It.IsAny<Stream>()))
                .ReturnsAsync(false);

            var sut = CreateServiceUnderTest();

            // act
            await sut.ProcessFileScanRequestAsync(request);

            // assert
            _responseQueueRepository.Verify(
                x => x.StoreMessageAsync(
                    It.Is<AntivirusScanResponse>(r => r.Response.Value.Equals(AntivirusResultHelper.VirusFoundCode))),
                        Times.Once);
        }

        [Fact]
        public async Task StartScanAsync_Should_StoreMessageInRequestQueue()
        {
            // arrange
            var request = _fixture.Create<AntivirusScanRequest>();
            var sut = CreateServiceUnderTest();

            // act
            await sut.StartScanAsync(request);

            // assert
            _requestQueueRepository.Verify(x => x.StoreMessageAsync(request), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_Should_ReturnTrue()
        {
            // arrange
            var response = new AntivirusScanResponse($"https://localhost/{_fixture.Create<string>()}",
                AntivirusResultHelper.CreateOkResult());

            var sut = CreateServiceUnderTest();

            // act
            var result = await sut.ProcessResponseAsync(response);

            // assert
            result.ShouldBeTrue();
        }

        private VirusScannerService CreateServiceUnderTest()
        {
            return new VirusScannerService(_requestQueueRepository.Object,
                _responseQueueRepository.Object,
                _cloudmersiveVirusScanner.Object,
                _httpClientFactory.Object);
        }
    }
}
