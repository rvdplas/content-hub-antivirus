using System.Threading.Tasks;
using AutoFixture;
using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;
using ContentHub.AntivirusScanner.Application.Services;
using Moq;
using Xunit;

namespace ContentHub.AntivirusScanner.Application.UnitTests.Services
{
    public class ScanRequestServiceTests
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IScanRequestQueueRepository> _repository;

        public ScanRequestServiceTests()
        {
            _repository = new Mock<IScanRequestQueueRepository>();
        }

        [Fact]
        public async Task StoreRequestAsync_Should_StoreRequestMessageInQueue()
        {
            // arrange
            var request = _fixture.Create<AntivirusScanRequest>();

            var sut = CreateServiceUnderTest();

            // act
            await sut.StoreRequestAsync(request);

            // assert
            _repository.Verify(x => x.StoreMessageAsync(request), Times.Once);
        }

        private ScanRequestService CreateServiceUnderTest()
        {
            return new ScanRequestService(_repository.Object);
        }
    }
}
