using System.Linq;
using System.Net.Http;
using ContentHub.AntivirusScanner.Application.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace ContentHub.AntivirusScanner.Functions.UnitTests
{
    public class StartupTests
    {
        [Fact]
        public void RegisterModules_Should_LoadAllModules()
        {
            // arrange
            var builder = new FakeBuilder();
            var sut = new Startup();

            // act
            sut.Configure(builder);

            // assert
            builder.Services.Any().ShouldBeTrue();
        }

        [Fact]
        public void RegisterModules_Should_RegisterFunctionTypes()
        {
            // arrange
            var builder = new FakeBuilder();
            var sut = new Startup();

            // act
            sut.Configure(builder);

            // assert
            builder.Services.First(x => x.ServiceType == typeof(IHttpClientFactory)).ShouldNotBeNull();
            builder.Services.First(x => x.ServiceType == typeof(ILogger)).ShouldNotBeNull();
        }

        [Fact]
        public void RegisterModules_Should_RegisterApplicationTypes()
        {
            // arrange
            var builder = new FakeBuilder();
            var sut = new Startup();

            // act
            sut.Configure(builder);

            // assert
            builder.Services.First(x => x.ServiceType == typeof(IVirusScannerService)).ShouldNotBeNull();
        }

        [Fact]
        public void RegisterModules_Should_RegisteCloudmersiveTypes()
        {
            // arrange
            var builder = new FakeBuilder();
            var sut = new Startup();

            // act
            sut.Configure(builder);

            // assert
            builder.Services.First(x => x.ServiceType == typeof(IOnlineScannerService)).ShouldNotBeNull();
        }

        [Fact]
        public void RegisterModules_Should_RegisteStorageTypes()
        {
            // arrange
            var builder = new FakeBuilder();
            var sut = new Startup();

            // act
            sut.Configure(builder);

            // assert
            builder.Services.First(x => x.ServiceType == typeof(IScanRequestQueueRepository)).ShouldNotBeNull();
            builder.Services.First(x => x.ServiceType == typeof(IScanResponseQueueRepository)).ShouldNotBeNull();
        }
    }

    public class FakeBuilder : IFunctionsHostBuilder
    {
        public IServiceCollection Services { get; }

        public FakeBuilder()
        {
            Services = new ServiceCollection();
        }
    }
}
