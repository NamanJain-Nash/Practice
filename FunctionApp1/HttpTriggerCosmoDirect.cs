using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.IServices;
using DatabaseModel;

namespace HttpFunctionApp
{
    public class HttpTriggerCosmoDirect
    {
        private readonly ILogger _log;
        private readonly IStoargeQueueService _stoargeQueue;
       private readonly IEventHubService _eventGrid;
        private readonly ICosmoDbService _cosmoDB;
        public HttpTriggerCosmoDirect(ILogger log,IStoargeQueueService stoargeQueue,ICosmoDbService cosmoDB,IEventHubService eventGridService)
        {
            _log = log;
            _stoargeQueue = stoargeQueue;
            _eventGrid=eventGridService;
            _cosmoDB = cosmoDB;
            
        }
        [FunctionName("HttpTriggerForMessage")]
        public async Task<IActionResult> MessageHandling(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

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
            
        }
        [FunctionName("HttpTriggerForEventGrid")]
        public async Task<IActionResult> EventGridMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

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
