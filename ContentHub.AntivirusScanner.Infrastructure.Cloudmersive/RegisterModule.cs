using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Infrastructure.Cloudmersive.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContentHub.AntivirusScanner.Infrastructure.Cloudmersive
{
    public static class RegisterModule
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            services.AddTransient<IOnlineScannerService, CloudmersiveVirusScanner>();
        }
    }
}