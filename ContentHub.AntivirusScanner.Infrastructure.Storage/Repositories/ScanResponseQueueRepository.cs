using ContentHub.AntivirusScanner.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContentHub.AntivirusScanner.Infrastructure.Storage.Repositories;

public class ScanResponseQueueRepository : QueueRepository, IScanResponseQueueRepository
{
    public const string QueueName = "scan-responses";

    public ScanResponseQueueRepository(ILogger logger, IConfiguration configuration) : base(logger, configuration, QueueName)
    {

    }
}