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

namespace EventGridFunctionApp
{
    public class EventGridFunction
    {
        private readonly IEventGridService _eventGrid;
        public EventGridFunction(IEventGridService _eventGrid) { }
        [FunctionName("TriggerByEventGrid")]
        public async Task<IActionResult> MessageHandlingForEventGrid(
            [EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = eventGridEvent.Data.ToString();

            // Deserialize the request body into MessagingModel
            MessagingModel message = JsonConvert.DeserializeObject<MessagingModel>(requestBody);

            // Validate if the message is null or empty
            if (message == null || string.IsNullOrEmpty(message.Name))
            {
                return new BadRequestObjectResult("Please provide valid data in the request body.");
            }

            // Send data to CosmoDbConnect class
            try
            {
                await _eventGrid.SendEventAsync(message);
                string responseMessage = $"Hello, {message.Name}. This HTTP triggered function executed successfully.";

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
