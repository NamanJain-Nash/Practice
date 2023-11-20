using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using DatabaseModel;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.IServices;

namespace EventHub
{
    public class EventHubTrigger
    {
        private readonly ILogger _log;
        private readonly IStoargeQueueService _stoargeQueue;
        private readonly ICosmoDbService _cosmoDB;
        public EventHubTrigger(ILogger log, IStoargeQueueService stoargeQueue, ICosmoDbService _cosmoDB)
        {
            _log = log;
            _stoargeQueue = stoargeQueue;
            _cosmoDB = _cosmoDB;

        }
        [FunctionName("EventHandler")]
        public async Task Run([EventHubTrigger("EventHubHandler", Connection = "")] EventData[] events)
        {
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.ToArray());
                    _log.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");

                    // Deserialize the message body into MessagingModel
                    MessageingModel message = JsonConvert.DeserializeObject<MessageingModel>(messageBody);

                    // Validate if the message is null or empty
                    if (message == null || string.IsNullOrEmpty(message.Id))
                    {
                        _log.LogError("Invalid data received in the message body.");
                        // Handle invalid message
                        continue; // Move to the next event
                    }

                    // Send data to CosmoDbConnect class
                    await _cosmoDB.SaveToCosmosDB(message);
                    string responseMessage = $"Hello, {message.Message}. This function executed successfully.";

                    _log.LogInformation(responseMessage);
                }
                catch (Exception ex)
                {
                    _log.LogError($"An error occurred: {ex.Message}");

                    // Log the error or send it to a storage queue or another service for further processing
                    await _stoargeQueue.SendMessageToQueueAsync(eventData, ex.Message);
                }
            }

        }
    }
}