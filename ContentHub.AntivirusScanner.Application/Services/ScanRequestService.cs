using ContentHub.AntivirusScanner.Application.Interfaces;
using ContentHub.AntivirusScanner.Application.Models;

namespace ContentHub.AntivirusScanner.Application.Services
{
    public class ScanRequestService
    {
        private readonly IScanRequestQueueRepository _queueRepository;

        public ScanRequestService(IScanRequestQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }

        public Task StoreRequestAsync(AntivirusScanRequest request)
        {
            return _queueRepository.StoreMessageAsync(request);
        }
    }
}
