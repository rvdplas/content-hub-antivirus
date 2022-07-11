namespace ContentHub.AntivirusScanner.Application.Models;

public class AntivirusScanResponse
{
    public AntivirusScanResponse(string callbackUrl, AntivirusScanResult response)
    {
        CallbackUrl = callbackUrl;
        Response = response;
    }

    public string CallbackUrl { get; }
    public AntivirusScanResult Response { get; }
}