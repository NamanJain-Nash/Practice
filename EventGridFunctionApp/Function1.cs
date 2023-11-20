// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Services.IServices;
using DatabaseModel;

namespace EventGridFunctionApp
{
    public class EventGridFunction
    {
        private readonly IStoargeQueueService _stoargeQueue;
        private readonly ICosmoDbService _cosmoDB;
        private readonly IEventGridService _eventGrid;
        private readonly ILogger _log;
        public EventGridFunction(IStoargeQueueService storageQueue,ICosmoDbService cosmoDbService, ILogger log, IEventGridService eventGrid)
        {
            _stoargeQueue = storageQueue;
            _cosmoDB = cosmoDbService;
            _log = log;
            _eventGrid = eventGrid;
        }
        [FunctionName("TriggerByEventGrid")]
        public async Task<IActionResult> MessageHandlingForEventGrid(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = eventGridEvent.Data.ToString();

            // Deserialize the request body into MessagingModel
            MessageingModel message = JsonConvert.DeserializeObject<MessageingModel>(requestBody);

            // Validate if the message is null or empty
            if (message == null || string.IsNullOrEmpty(message.Id))
            {
                return new BadRequestObjectResult("Please provide valid data in the request body.");
            }

            // Send data to CosmoDbConnect class
            try
            {
                await _cosmoDB.SaveToCosmosDB(message);
                string responseMessage = $"Hello, {message.Message}. This HTTP triggered function executed successfully.";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                await _stoargeQueue.SendMessageToQueueAsync(message,ex.Message);
                    return new OkObjectResult(ex.Message);
            }
            message = JsonConvert.DeserializeObject<MessageingModel>(requestBody);

            // Validate if the message is null or empty
            if (message == null || string.IsNullOrEmpty(message.Id))
            {
                return new BadRequestObjectResult("Please provide valid data in the request body.");
            }

            // Send data to CosmoDbConnect class
            try
            {
                await _eventGrid.SendEventAsync(message);
                string responseMessage = $"Hello, {message.Message}. This HTTP triggered function executed successfully.";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                await _stoargeQueue.SendMessageToQueueAsync(message, ex.Message);
                return new OkObjectResult(ex.Message);
            }
        }
    }
}
