namespace ContentHub.AntivirusScanner.Application.Models
{
    public class AntivirusScanResult
    {
        public AntivirusScanResult(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}