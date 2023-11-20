using Azure.Messaging.EventHubs.Producer;
using DatabaseModel;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Services.Services
{
    
    public   class EventHubService: IEventHubService
    {
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private readonly EventHubProducerClient _clinet;
        public EventHubService(IConfiguration config, ILogger log)
        {
            _config = config;
            _log = log;
            _clinet = new EventHubProducerClient(
                _config["EventHubConnectionString"], _config["EventHubName"]);
        }
        public async Task<string> SendEventAsync(MessageingModel eventData)
        {

            //We will be making the event batch
            using EventDataBatch eventBatch = await _clinet.CreateBatchAsync();
            string message =JsonConvert.SerializeObject(eventData);
            //adition of the data
            if (!eventBatch.TryAdd(new Azure.Messaging.EventHubs.EventData(Encoding.UTF8.GetBytes(message))))
            {
                return "Event additon of data failed due to message";
            }
            try
            {
                await _clinet.SendAsync(eventBatch);
                return "Event Send Succesfully";
            }

            catch (Exception ex)
            {
                return "Event Send Failed due to " + ex.Message;
            }
        }

    }
}
