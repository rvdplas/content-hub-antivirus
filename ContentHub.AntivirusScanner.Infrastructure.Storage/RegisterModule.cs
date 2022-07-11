using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Infrastructure.Storage.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ContentHub.AntivirusScanner.Infrastructure.Storage
{
    public static class RegisterModule
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            services.AddTransient<IScanRequestQueueRepository, ScanRequestQueueRepository>();
            services.AddTransient<IScanResponseQueueRepository, ScanResponseQueueRepository>();
        }
    }
}