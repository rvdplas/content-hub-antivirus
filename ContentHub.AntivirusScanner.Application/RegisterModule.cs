using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContentHub.AntivirusScanner.Application
{
    public static class RegisterModule
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            services.AddTransient<IVirusScannerService, VirusScannerService>();
        }
    }
}