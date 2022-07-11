using ContentHub.AntivirusScanner.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentHub.AntivirusScanner.Infrastructure.Storage.Repositories
{
    public class ScanRequestQueueRepository : QueueRepository, IScanRequestQueueRepository
    {
        public const string QueueName = "scan-requests";

        public ScanRequestQueueRepository(ILogger logger, IConfiguration configuration) : base(logger, configuration, QueueName)
        {

        }
    }
}