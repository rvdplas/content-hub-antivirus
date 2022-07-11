using ContentHub.AntivirusScanner.Application.Models;

namespace ContentHub.AntivirusScanner.Application.Extensions
{
    public static class AntivirusScanRequestExtensions
    {
        public static bool IsValid(this AntivirusScanRequest request)
        {
            if (string.IsNullOrEmpty(request.CallbackUrl))
            {
                return false;
            }

            if (!request.Sources.Any())
            {
                return false;
            }

            try
            {
                var _ = new Uri(request.CallbackUrl, UriKind.Absolute);

                foreach (var sourceUrl in request.Sources)
                {
                    _ = new Uri(sourceUrl, UriKind.Absolute);
                }
            }
            catch (UriFormatException)
            {
                return false;
            }

            return true;
        }
    }
}
