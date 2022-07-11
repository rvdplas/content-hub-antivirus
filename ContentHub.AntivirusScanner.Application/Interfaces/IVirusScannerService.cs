using ContentHub.AntivirusScanner.Application.Models;

namespace ContentHub.AntivirusScanner.Application.Interfaces;

public interface IVirusScannerService
{
    Task StartScanAsync(AntivirusScanRequest antivirusScanRequest);
    Task ProcessFileScanRequestAsync(AntivirusScanRequest request);
    Task<bool> ProcessResponseAsync(AntivirusScanResponse response);
}