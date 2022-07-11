using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ContentHub.AntivirusScanner.Infrastructure.Storage.Repositories
{
    public abstract class QueueRepository
    {
        private const string QueueConnectionstring = "QueueConnectionstring";


        private const int MaximumMessages = 2;
        private readonly TimeSpan _maxTimeSpanTimeout = TimeSpan.FromMinutes(4);

        protected readonly QueueClient QueueClient;
        private readonly ILogger _logger;

        protected QueueRepository(ILogger logger, IConfiguration configuration, string queueName)
        {
            _logger = logger;

            var queueConnectionString = configuration[QueueConnectionstring];
            if (string.IsNullOrEmpty(queueConnectionString))
            {
                _logger.LogError($"{nameof(queueConnectionString)} is empty");
                throw new ArgumentNullException(nameof(queueConnectionString));
            }

            if (string.IsNullOrEmpty(queueName))
            {
                _logger.LogError($"{nameof(queueName)} is empty");
                throw new ArgumentNullException(nameof(queueName));
            }

            QueueClient = new QueueClient(queueConnectionString, queueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64,
            });
            Initialize();
        }

        public void Initialize()
        {
            if (!QueueClient.Exists())
            {
                QueueClient.Create();
                _logger.LogInformation($"Queue {QueueClient.Name} has been created.");
            }
            else
            {
                _logger.LogInformation($"Queue {QueueClient.Name} does exists.");
            }
        }

        public Task StoreMessageAsync<T>(T obj) where T : class
        {
            var json = JsonConvert.SerializeObject(obj);
            return QueueClient.SendMessageAsync(json);
        }

        public async Task<QueueMessage[]> ReadMessagesAsync()
        {
            var response = await QueueClient.ReceiveMessagesAsync(MaximumMessages, _maxTimeSpanTimeout);
            return response.Value;
        }

        public Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            return QueueClient.DeleteMessageAsync(messageId, popReceipt);
        }
    }
}
