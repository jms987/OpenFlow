using Azure.Storage.Queues.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;

namespace OpenFlowWebServer.Repository
{
    public interface IQueueRepository
    {
        // Define methods for the queue repository here
        // For example:
        Task EnqueueAsync(List<QueueItem> item,string queueName);
        Task EnqueueAsync(QueueItem item, string queueName);
        Task EnqueueAsync(Object item, string queueName);
        Task EnqueueAsync(IEnumerable<object> item, string queueName);
       // Task<QueueItem> DequeueAsync(string queueName);
        Task<Dictionary<string, object>?> DequeueAsync(string queueName);
        Task<int> GetCountAsync( string queueName);
        Task<int> GetCountAsync( Guid projectGuid);
        Task DeleteQueueAsync(string queueName);
    }

    public class QueueRepository : IQueueRepository
    {

        private readonly QueueServiceClient _queueService;
        public QueueRepository(QueueServiceClient queueService)
        {
            _queueService = queueService;
        }

        public async Task EnqueueAsync(IEnumerable<object> items, string queueName)
        {
            foreach (var item in items)
            {
                await EnqueueAsync(item, queueName);
            }
        }

        public async Task EnqueueAsync(List<QueueItem> items, string queueName)
        {
            foreach (var item in items)
            {
                await EnqueueAsync(item,queueName);
            }
        }

        public async Task EnqueueAsync(QueueItem item, string queueName)
        {
            var message = JsonSerializer.Serialize(item);
            var queueClient = _queueService.GetQueueClient(queueName.ToLower());
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }

        public async Task EnqueueAsync(object item, string queueName)
        {
            var message = JsonSerializer.Serialize(item);
            var queueClient = _queueService.GetQueueClient(queueName.ToLower());
            await queueClient.CreateIfNotExistsAsync();
            try
            {
                var response = await queueClient.SendMessageAsync(message);
                var a = 0;
                if (response.GetRawResponse().IsError)
                {
                    var b = 1;
                }
            }
            catch (Exception e)
            {
                var c = 0;
            }
        }

        //public async Task<QueueItem> DequeueAsync(string queueName)
        public async Task<Dictionary<string, object>?> DequeueAsync(string queueName)
        {
            var queueClient = _queueService.GetQueueClient(queueName.ToLower());
            var response = await queueClient.ReceiveMessagesAsync(1);
            var message = response.Value.FirstOrDefault();
            if (message == null)
                return null;
           // var queueItem = JsonSerializer.Deserialize<QueueItem>(message.MessageText);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(message.MessageText);
            await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            return dict;
        }

        public async Task<int> GetCountAsync( string queueName)
        {
            var queueClient = _queueService.GetQueueClient(queueName.ToLower());
            var properties = await queueClient.GetPropertiesAsync();
            return properties.Value.ApproximateMessagesCount;
            //return queueElemnts;
        }

        public Task<int> GetCountAsync(Guid projectGuid)
        {
            return GetCountAsync($"ProjectId{projectGuid.ToString()}Parameters");
        }

        public async Task DeleteQueueAsync(string queueName)
        {
            var queueClient = _queueService.GetQueueClient(queueName.ToLower());
            await queueClient.DeleteIfExistsAsync();
        }
    }
}
    