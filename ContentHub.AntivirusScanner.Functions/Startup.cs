using ContentHub.AntivirusScanner.Functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ContentHub.AntivirusScanner.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSingleton<ILogger, Logger<AntivirusFunctions>>();

            Application.RegisterModule.RegisterComponents(builder.Services);
            Infrastructure.Cloudmersive.RegisterModule.RegisterComponents(builder.Services);
            Infrastructure.Storage.RegisterModule.RegisterComponents(builder.Services);
        }
    }
}