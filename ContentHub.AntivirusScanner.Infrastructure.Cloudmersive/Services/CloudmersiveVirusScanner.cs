using Cloudmersive.APIClient.NETCore.VirusScan.Api;
using Cloudmersive.APIClient.NETCore.VirusScan.Client;
using ContentHub.AntivirusScanner.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentHub.AntivirusScanner.Infrastructure.Cloudmersive.Services
{
    public class CloudmersiveVirusScanner : IOnlineScannerService
    {
        private const string CloudmersiveApiKey = "Cloudmersive-ApiKey";

        private readonly ScanApi _apiInstance;
        private readonly ILogger _logger;

        public CloudmersiveVirusScanner(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;

            var apiKey = configuration[CloudmersiveApiKey];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            Configuration.Default.AddApiKey("Apikey", apiKey);
            _apiInstance = new ScanApi();
        }

        public async Task<bool> IsFileVirusFree(Stream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream), "FileStream is required.");
            }

            try
            {
                var result = await _apiInstance.ScanFileAsync(fileStream);
                return result.CleanResult.HasValue && result.CleanResult.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
