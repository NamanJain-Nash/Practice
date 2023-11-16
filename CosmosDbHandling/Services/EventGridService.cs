using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    
    public   class EventGridService: IEventGridService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly string TopicEndpointUri ;
        private readonly string TopicKey ;
        private readonly EventGridClient client;
        public EventGridService(ILogger logger,IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            TopicKey = _configuration["EventGridKey"];
            TopicEndpointUri = _configuration["EventGridUri"];
            TopicCredentials credentials = new TopicCredentials(TopicKey);
            client = new EventGridClient(credentials);
        }
        public async Task<string> SendEventAsync(MessageingModel eventData)
        {
            

            var events = new List<EventGridEvent>
        {
            new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                EventType = "YourCustomEventType",
                Data = eventData,
                EventTime = DateTime.UtcNow,
                Subject = "YourCustomSubject",
                DataVersion = "1.0"
            }
        };

            await client.PublishEventsAsync(TopicEndpointUri, events);
            return "";
        }

    }
}
