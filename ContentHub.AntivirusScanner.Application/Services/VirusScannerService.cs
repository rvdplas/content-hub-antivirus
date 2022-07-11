using System.Text;
using ContentHub.AntivirusScanner.Application.Helpers;
using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;
using Newtonsoft.Json;

namespace ContentHub.AntivirusScanner.Application.Services;

public class VirusScannerService : IVirusScannerService
{
    private readonly IScanRequestQueueRepository _requestQueueRepository;
    private readonly IScanResponseQueueRepository _responseQueueRepository;
    private readonly IOnlineScannerService _cloudmersiveVirusScanner;
    private readonly IHttpClientFactory _httpClientFactory;

    public VirusScannerService(IScanRequestQueueRepository requestQueueRepository,
        IScanResponseQueueRepository responseQueueRepository,
        IOnlineScannerService cloudmersiveVirusScanner,
        IHttpClientFactory httpClientFactory)
    {
        _requestQueueRepository = requestQueueRepository;
        _responseQueueRepository = responseQueueRepository;
        _cloudmersiveVirusScanner = cloudmersiveVirusScanner;
        _httpClientFactory = httpClientFactory;
    }

    public Task StartScanAsync(AntivirusScanRequest antivirusScanRequest)
    {
        return _requestQueueRepository.StoreMessageAsync(antivirusScanRequest);
    }

    public async Task<bool> ProcessResponseAsync(AntivirusScanResponse response)
    {
        var content =
            new StringContent(JsonConvert.SerializeObject(response.Response), Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient();
        var postResult = await client.PostAsync(response.CallbackUrl, content);

        return postResult.IsSuccessStatusCode;
    }

    public async Task ProcessFileScanRequestAsync(AntivirusScanRequest request)
    {
        var isVirusFree = true;
        foreach (var fileSource in request.Sources)
        {
            if (await ScanFileAsync(fileSource))
            {
                continue;
            }

            isVirusFree = false;
            break;
        }

        var response = new AntivirusScanResponse(request.CallbackUrl, CreateAntivirusScanResponse(isVirusFree));
        await _responseQueueRepository.StoreMessageAsync(response);
    }

    private static AntivirusScanResult CreateAntivirusScanResponse(bool isVirusFree)
    {
        return isVirusFree
            ? AntivirusResultHelper.CreateOkResult()
            : AntivirusResultHelper.CreateVirusFoundResult();
    }

    private async Task<bool> ScanFileAsync(string fileUrl)
    {
        var client = _httpClientFactory.CreateClient();

        await using var stream = await client.GetStreamAsync(fileUrl);
        return await _cloudmersiveVirusScanner.IsFileVirusFree(stream);
    }
}