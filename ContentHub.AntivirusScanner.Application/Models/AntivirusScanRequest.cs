namespace ContentHub.AntivirusScanner.Application.Models
{
    public class AntivirusScanRequest
    {
        public AntivirusScanRequest(string callbackUrl, List<string> sources)
        {
            CallbackUrl = callbackUrl;
            Sources = sources;
        }

        public string CallbackUrl { get; }
        public List<string> Sources { get; }
    }
}