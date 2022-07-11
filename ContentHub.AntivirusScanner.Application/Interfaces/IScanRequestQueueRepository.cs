using Azure.Storage.Queues.Models;

namespace ContentHub.AntivirusScanner.Application.Interfaces;

public interface IScanRequestQueueRepository
{
    Task StoreMessageAsync<T>(T obj) where T : class;
    Task<QueueMessage[]> ReadMessagesAsync();
    Task DeleteMessageAsync(string messageId, string popReceipt);
}