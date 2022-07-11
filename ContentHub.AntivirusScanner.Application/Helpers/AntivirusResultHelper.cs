using ContentHub.AntivirusScanner.Application.Models;

namespace ContentHub.AntivirusScanner.Application.Helpers
{
    public static class AntivirusResultHelper
    {
        public const string VirusFreeCode = "Ok";
        public const string VirusFoundCode = "Malicious";

        public static AntivirusScanResult CreateOkResult()
        {
            return new AntivirusScanResult(VirusFreeCode);
        }

        public static AntivirusScanResult CreateVirusFoundResult()
        {
            return new AntivirusScanResult(VirusFoundCode);
        }
    }
}