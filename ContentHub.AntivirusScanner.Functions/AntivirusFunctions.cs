using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using ContentHub.AntivirusScanner.Application.Extensions;
using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;
using ContentHub.AntivirusScanner.Infrastructure.Storage.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContentHub.AntivirusScanner.Functions
{
    public class AntivirusFunctions
    {
        private readonly IVirusScannerService _virusScannerService;
        private readonly ILogger _logger;

        public AntivirusFunctions(IVirusScannerService virusScannerService, ILogger logger)
        {
            _virusScannerService = virusScannerService;
            _logger = logger;
        }

        [FunctionName("ReceiveScanRequest")]
        public async Task<IActionResult> ReceiveScanRequest(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest request)
        {
            try
            {
                if (request?.Body == null)
                {
                    throw new InvalidOperationException("Empty payload.");
                }

                var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                var antivirusScanRequest = JsonConvert.DeserializeObject<AntivirusScanRequest>(requestBody);

                if (!antivirusScanRequest.IsValid())
                {
                    throw new InvalidOperationException("Invalid payload.");
                }

                await _virusScannerService.StartScanAsync(antivirusScanRequest);

                return new OkResult();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new InternalServerErrorResult();
            }
        }
        
        [FunctionName("ProcessScanRequest")]
        public async Task ProcessScanRequest([QueueTrigger(ScanRequestQueueRepository.QueueName, Connection = "QueueConnectionstring")] string queueItemString)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<AntivirusScanRequest>(queueItemString);
                await _virusScannerService.ProcessFileScanRequestAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        
        [FunctionName("ProcessScanResult")]
        public async Task ProcessScanResult([QueueTrigger(ScanResponseQueueRepository.QueueName, Connection = "QueueConnectionstring")] string queueItemString)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<AntivirusScanResponse>(queueItemString);
                await _virusScannerService.ProcessResponseAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
