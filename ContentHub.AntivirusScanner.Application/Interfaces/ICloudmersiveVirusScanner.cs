namespace ContentHub.AntivirusScanner.Application.Interfaces;

public interface IOnlineScannerService
{
    Task<bool> IsFileVirusFree(Stream fileStream);
}