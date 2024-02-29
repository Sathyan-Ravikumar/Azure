using System;
using System.Text.RegularExpressions;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions
{
    public class GetMessage
    {
        private readonly ILogger<GetMessage> _logger;

        public GetMessage(ILogger<GetMessage> logger)
        {
            _logger = logger;
        }

        [Function(nameof(GetMessage))]
        public void Run([QueueTrigger("queue1", Connection = "QueueConnection")] QueueMessage message)
        {
            if (!Regex.IsMatch(message.MessageText, @"^[a-zA-Z]+$"))
            {
                //throw new Exception("Invalid message format. Message should be a string containing only letters (no numbers or special characters).");
                _logger.LogError("Invalid message format. Message should be a string containing only letters (no numbers or special characters).");
            }
            else
            {
                _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
            }
        }

    }
}
