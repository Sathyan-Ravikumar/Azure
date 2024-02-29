using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GetMessagesFromQueue
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Function1))]
        public void Run([ServiceBusTrigger("firstqueue", Connection = "ConnectionString")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
            var data = JsonSerializer.Deserialize<Data>(message.Body.ToString());
           
            var addedDate = data.Date.AddSeconds(30);

            if (data.Date > addedDate)
            {
                _logger.LogInformation($"Valid date: {data}");
            }
            else
            {
                _logger.LogInformation("expired");
            }
        }
    }
}
