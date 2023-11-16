using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class StoargeQueueService: IStoargeQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<StoargeQueueService> _logger;
        private readonly IConfiguration _configuration;
        public StoargeQueueService(IConfiguration configuration, ILogger<StoargeQueueService> logger)
        {
            _configuration = configuration;
            string connectionString = _configuration["QueueConnectionString"];
            string queueName = configuration["QueueName"];

            _queueClient = new QueueClient(connectionString, queueName);
            _logger = logger;
        }

        public QueueClient Get_queueClient<T>()
        {
            return _queueClient;
        }

        public async Task<string> SendMessageToQueueAsync<MessageingModel>(MessageingModel data,string error)
        {
            try
            {
                string jsonData = JsonSerializer.Serialize(data)+"\nError"+error;
                Message message = new Message() { Body= Encoding.UTF8.GetBytes(jsonData+"  -  "+ DateTime.UtcNow) };
                //message = new Message(Encoding.UTF8.GetBytes(jsonData))??new Message();
                await _queueClient.SendAsync(message);
                return "Completed";
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return ex.Message;
            }
        }
    }
}
